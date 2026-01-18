using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using BackEnd.BackndNewtonsoft.Json;

public class BackendManager : Singleton<BackendManager>
{
    public PUserInfo userInfo = new PUserInfo();
    public PDungeonInfo dungeonInfo = new PDungeonInfo();
    public PTownInfo townInfo = new PTownInfo();
    public PBuildingSaveData buildingSaveData = new PBuildingSaveData();

    public Subject<bool> OnInitBackEnd = new Subject<bool>();
    public Subject<bool> OnBackEndLogin = new Subject<bool>();
    public Subject<EGameData> OnGetLoadData = new Subject<EGameData>();

    private BackendReturnObject backendInit = null;


    public void InitBackEnd()
    {
        if (backendInit != null)
            return;

        backendInit = Backend.Initialize();

        if (backendInit.IsSuccess())
        {
            if (GetComponent<SendQueueMgr>() == null)
                gameObject.AddComponent<SendQueueMgr>();

            Debug.Log("[Backend] Initialize success");
            OnInitBackEnd.OnNext(true);
        }
        else
        {
            Debug.LogError("[Backend] Initialize failed: " + backendInit);
            OnInitBackEnd.OnNext(false);
        }
    }

    public void LoginGuest()
    {
        SendQueue.Enqueue(Backend.BMember.GuestLogin, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("[Backend] GuestLogin failed: " + callback);
                OnBackEndLogin.OnNext(false);
                return;
            }

            // 로그인 성공 후 유저 정보 가져오기
            LoginGetUserInfo();
        });
    }

    private void LoginGetUserInfo()
    {
        // 동기식: 간단하고 확실 (MVP에 적합)
        var bro = Backend.BMember.GetUserInfo();

        if (bro == null)
        {
            Debug.LogError("[Backend] GetUserInfo bro is null");
            OnBackEndLogin.OnNext(false);
            return;
        }

        if (!bro.IsSuccess())
        {
            Debug.LogError("[Backend] GetUserInfo failed: " + bro);
            OnBackEndLogin.OnNext(false);
            return;
        }

        var json = bro.GetReturnValuetoJSON();
        if (json == null)
        {
            Debug.LogError("[Backend] GetReturnValuetoJSON() is null");
            OnBackEndLogin.OnNext(false);
            return;
        }

        if (!json.ContainsKey("row") || json["row"] == null)
        {
            Debug.LogWarning("[Backend] GetUserInfo has no row. First login?");
            // 👉 이 경우에도 로그인 자체는 성공으로 봄
            OnBackEndLogin.OnNext(true);
            return;
        }

        JsonData row = json["row"];

        if (row.ContainsKey("nickname") && row["nickname"] != null)
            UserManager.Ins.userNickName = row["nickname"].ToString();

        if (row.ContainsKey("inDate") && row["inDate"] != null)
            UserManager.Ins.userIndate = row["inDate"].ToString();

        if (row.ContainsKey("gamerId") && row["gamerId"] != null)
            UserManager.Ins.userGamerID = row["gamerId"].ToString();


        OnBackEndLogin.OnNext(true);
    }

    public void LoadAll()
    {
        foreach (EGameData e in Enum.GetValues(typeof(EGameData)))
            Load(e);
    }

    public void Load(EGameData eGameData)
    {
        SendQueue.Enqueue(Backend.GameData.GetMyData, eGameData.ToString(), new Where(), 1, bro =>
        {
            if (!bro.IsSuccess())
            {
                Debug.LogError($"[Backend] GetMyData fail: {eGameData} / {bro}");
                return;
            }

            JsonData rows = bro.GetFlattenJSON()["rows"];

            // 저장된 것이 없으면 기본값 Insert
            if (rows.Count <= 0)
            {
                var insertParam = GetParamGameData(eGameData);
                SendQueue.Enqueue(Backend.GameData.Insert, eGameData.ToString(), insertParam, insertCallback =>
                {
                    if (!insertCallback.IsSuccess())
                    {
                        Debug.LogError($"[Backend] Insert fail: {eGameData} / {insertCallback}");
                        return;
                    }

                    // Insert 완료 후 로컬 캐시 동기화 (기본값 저장됨)
                    CacheSnapshot(eGameData);

                    OnGetLoadData.OnNext(eGameData);
                    Debug.Log($"[Backend] Insert Done: {eGameData}");
                });

                return;
            }

            // 저장된 값 로컬 반영
            ApplyLoadedData(eGameData, rows[0]);

            // “현재 상태”를 스냅샷으로 캐싱해서 IsDifferent 비교 기준으로 삼는다
            CacheSnapshot(eGameData);

            OnGetLoadData.OnNext(eGameData);
            Debug.Log($"[Backend] Load Done: {eGameData}");
        });
    }

    private void ApplyLoadedData(EGameData eGameData, JsonData row0)
    {
        string json = row0.ToJson();
        switch (eGameData)
        {
            case EGameData.UserInfo:
            {
                var data = JsonConvert.DeserializeObject<PUserInfo>(json);
                UserManager.Ins.userInfo = data;
                userInfo = data;
            }
            break;

            case EGameData.DungeonInfo:
            {
                var data = JsonConvert.DeserializeObject<PDungeonInfo>(json);
                UserManager.Ins.dungeonInfo = data;
                dungeonInfo = data;
            }
            break;

            case EGameData.TownInfo:
            {
                var data = JsonConvert.DeserializeObject<PTownInfo>(json);
                UserManager.Ins.townInfo = data;
                townInfo = data;
            }
            break;

            case EGameData.BuildingInfo:
            {
                var data = JsonConvert.DeserializeObject<PBuildingSaveData>(json);
                    if (data == null) data = new PBuildingSaveData();

                    data.SyncFromJson();

                    UserManager.Ins.buildingSaveData = data;
                    buildingSaveData = data;
                }
            break;

            default:
                // 네 프로젝트에서 다른 데이터가 있으면 여기에 추가
                break;
        }
    }

    public void SaveData()
    {
        // 변경된 데이터만 모아서 TransactionWriteV2로 저장
        List<EGameData> changed = new List<EGameData>();
        foreach (EGameData e in Enum.GetValues(typeof(EGameData)))
        {
            if (IsDifferent(e))
                changed.Add(e);
        }

        if (changed.Count == 0)
            return;

        List<TransactionValue> transactionList = new List<TransactionValue>();
        foreach (var e in changed)
        {
            transactionList.Add(TransactionValue.SetUpdate(
                e.ToString(),
                new Where(),
                GetParamGameData(e)
            ));
        }

        // 배치(10개씩) 순차 저장
        const int batchSize = 10;
        int total = transactionList.Count;

        Action<int> sendNext = null;
        sendNext = (startIndex) =>
        {
            int remain = total - startIndex;
            int count = Mathf.Min(batchSize, remain);

            var subTransactions = transactionList.GetRange(startIndex, count);
            var subChanged = changed.GetRange(startIndex, count);

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, subTransactions, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError("[Backend] Save(TransactionWriteV2) fail: " + callback);
                    return;
                }

                // 저장 성공한 것만 캐싱 갱신
                foreach (var e in subChanged)
                {
                    CacheSnapshot(e);
                    Debug.Log($"[Backend] Save Done: {e}");
                }

                int next = startIndex + count;
                if (next < total)
                    sendNext(next);
            });
        };

        sendNext(0);
    }
    private bool IsDifferent(EGameData eGameData)
    {
        switch (eGameData)
        {
            case EGameData.UserInfo:
                return UserManager.Ins.userInfo.IsDifferent(userInfo);

            case EGameData.DungeonInfo:
                return UserManager.Ins.dungeonInfo.IsDifferent(dungeonInfo);

            case EGameData.TownInfo:
                return UserManager.Ins.townInfo.IsDifferent(townInfo);

            case EGameData.BuildingInfo:
                return UserManager.Ins.buildingSaveData.IsDifferent(buildingSaveData);

        }

        return false;
    }

    private void CacheSnapshot(EGameData eGameData)
    {
        switch (eGameData)
        {
            case EGameData.UserInfo:
                userInfo = CloneUtil.DeepClone(UserManager.Ins.userInfo);
                break;

            case EGameData.DungeonInfo:
                dungeonInfo = CloneUtil.DeepClone(UserManager.Ins.dungeonInfo);
                break;

            case EGameData.TownInfo:
                townInfo = CloneUtil.DeepClone(UserManager.Ins.townInfo);
                break;

            case EGameData.BuildingInfo:
                buildingSaveData = CloneUtil.DeepClone(UserManager.Ins.buildingSaveData);
                break;

        }
    }

    private Param GetParamGameData(EGameData eGameData)
    {
        // 너 프로젝트의 PXXX.FormatJsonData() 그대로 사용
        switch (eGameData)
        {
            case EGameData.UserInfo:
                return UserManager.Ins.userInfo.FormatJsonData();

            case EGameData.DungeonInfo:
                return UserManager.Ins.dungeonInfo.FormatJsonData();

            case EGameData.TownInfo:
                return UserManager.Ins.townInfo.FormatJsonData();

            case EGameData.BuildingInfo:
                return UserManager.Ins.buildingSaveData.FormatJsonData();

        }

        Debug.LogError($"[Backend] GetParamGameData Error: {eGameData}");
        return new Param();
    }


    //// Start is called before the first frame update
    //void Start()
    //{
    //    var bro = Backend.Initialize(); // 뒤끝 초기화

    //    // 뒤끝 초기화에 대한 응답값
    //    if (bro.IsSuccess())
    //    {
    //        Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
    //    }
    //    else
    //    {
    //        Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
    //    }
    
    //}

}
