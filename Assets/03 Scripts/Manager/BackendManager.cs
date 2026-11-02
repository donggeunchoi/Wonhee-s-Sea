using BackEnd;
using BackEnd.BackndNewtonsoft.Json;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackendManager : Singleton<BackendManager>
{
    PuserInfo userInfo = new PuserInfo();
    PDungeonInfo dungeonInfo = new PDungeonInfo();
   
    [Header("뒤끝 차트 고정 데이터")]
    public List<FishData> fishDatas = new List<FishData>();
    public List<ItemDataTable> itemDataTables = new List<ItemDataTable>();
    public List<StandData> standDatas = new List<StandData>();
    public List<EquipmentData> equipmentDatas = new List<EquipmentData>();
    public List<IngredientData> ingredientDatas = new List<IngredientData>();
    public List<StandRecipeData> standRecipeDatas = new List<StandRecipeData>();
    public List<StandRecipeInputData> standRecipeInputDatas = new List<StandRecipeInputData>();
    public List<DisplayStandData> displayStandDatas = new List<DisplayStandData>();
    public List<StoreData> storeDatas = new List<StoreData>();

    public Subject<bool> OnInitBackEnd = new Subject<bool>();
    public Subject<bool> OnBackEndLogin = new Subject<bool>();
    public Subject<EGameData> OnGetLoadData = new Subject<EGameData>();
    public Subject<bool> OnLoadAllChartData = new Subject<bool>();
    public Subject<bool> OnLoadChartItem = new Subject<bool>();
    public Subject<bool> test = new Subject<bool>();

    //던전에서 경영 넘어올때 던져주는 데이터
    public Subject<string> OnTossFish = new Subject<string>();

    private BackendReturnObject backendInit = null;

    [Header("Chart File Id")]
    [SerializeField] private string fishChartFileId = "";
    [SerializeField] private string itemDataTableChartFileId = "";
    [SerializeField] private string standChartFileId = "";
    [SerializeField] private string equipmentChartFileId = "";
    [SerializeField] private string ingredientChartFileId = "";
    [SerializeField] private string standRecipeChartFileId = "";
    [SerializeField] private string standRecipeInputChartFileId = "";
    [SerializeField] private string displayStandChartFileId = "";
    [SerializeField] private string storeDataId = "";

    public Subject<bool> OnChangedLoginGoogle = new Subject<bool>();

    public bool IsBackendLoggedIn { get; private set; }
    public bool IsGuestAccount { get; private set; }

    public string LastLoginError { get; private set; } = "";
    public string LastGoogleLinkError { get; private set; } = "";

    private bool isAuthRequestInProgress;
    private bool isGoogleLinkInProgress;

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


    // 구글 로그인
    public void LoginGoogle()
    {
        if (!BeginAuthentication()) return;

        RequestGoogleToken(false, (bool isSuccess, string errorMessage, string token) =>
        {
            if (!isSuccess || string.IsNullOrEmpty(token))
            {
                FailAuthentication("Google authentication failed: " + errorMessage);
                return;
            }

            AuthorizeGoogleToken(token);
        });
    }

    // 외부에서 구글 토큰을 전달받는 방식
    public void LoginGoogle(string token)
    {
        if (!BeginAuthentication()) return;

        AuthorizeGoogleToken(token);
    }

    // 로그인 중복 요청 방지
    private bool BeginAuthentication()
    {
        if (isAuthRequestInProgress || isGoogleLinkInProgress)
        {
            Debug.LogWarning("[Backend] Authentication is already in progress.");
            return false;
        }

        if (IsBackendLoggedIn)
        {
            Debug.LogWarning("[Backend] Already logged in.");
            return false;
        }

        LastLoginError = "";
        isAuthRequestInProgress = true;
        return true;
    }

    // 구글 토큰을 이용한 뒤끝 로그인
    private void AuthorizeGoogleToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            FailAuthentication("Google token is empty.");
            return;
        }

        SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, callback =>
        {
            if (callback == null || !callback.IsSuccess())
            {
                FailAuthentication("Google federation failed: " + callback);
                return;
            }

            IsGuestAccount = false;

            Debug.Log("[Backend] Google federation success.");

            LoginGetUserInfo();
        });
    }

    // 게스트 로그인
    public void LoginGuest()
    {
        if (!BeginAuthentication()) return;

        SendQueue.Enqueue(Backend.BMember.GuestLogin, callback =>
        {
            if (callback == null || !callback.IsSuccess())
            {
                FailAuthentication("GuestLogin failed: " + callback);
                return;
            }

            IsGuestAccount = true;

            Debug.Log("[Backend] Guest login success.");

            LoginGetUserInfo();
        });
    }

    // 게스트 계정을 구글 계정으로 연동
    public void LinkGuestToGoogle()
    {
        if (!BeginGoogleLink()) return;

        RequestGoogleToken(true, (bool isSuccess, string errorMessage, string token) =>
        {
            if (!isSuccess || string.IsNullOrEmpty(token))
            {
                FinishGoogleLink(false, "Google authentication for linking failed: " + errorMessage);
                return;
            }

            ChangeGuestToGoogle(token);
        });
    }

    // 이미 구글 토큰을 가지고 있는 경우 연동
    public void LinkGuestToGoogle(string token)
    {
        if (!BeginGoogleLink()) return;

        ChangeGuestToGoogle(token);
    }

    // 계정 연동 시작 조건 확인
    private bool BeginGoogleLink()
    {
        if (isAuthRequestInProgress || isGoogleLinkInProgress)
        {
            Debug.LogWarning("[Backend] Another authentication operation is in progress.");
            return false;
        }

        if (!IsBackendLoggedIn || !IsGuestAccount)
        {
            FinishGoogleLink(false, "A guest account must be logged in before linking.");
            return false;
        }

        LastGoogleLinkError = "";
        isGoogleLinkInProgress = true;

        return true;
    }

    // 게스트에서 구글 계정으로 변경
    private void ChangeGuestToGoogle(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            FinishGoogleLink(false, "Google token for linking is empty.");
            return;
        }

        SendQueue.Enqueue(Backend.BMember.ChangeCustomToFederation, token, FederationType.Google, callback =>
        {
            if (callback == null || !callback.IsSuccess())
            {
                string errorMessage = callback == null ? "Empty BACKND response." : callback.ToString();

                if (callback != null && callback.GetStatusCode() == "409")
                    errorMessage = "This Google account is already connected to another account. " + errorMessage;

                FinishGoogleLink(false, errorMessage);
                return;
            }

            IsGuestAccount = false;

            // 연동 성공 후 기존 로컬 게스트 인증 정보 삭제
            Backend.BMember.DeleteGuestInfo();

            Debug.Log("[Backend] Guest account linked to Google successfully.");

            FinishGoogleLink(true, "");
        });
    }

    // 구글 계정 연동 결과 전달
    private void FinishGoogleLink(bool success, string errorMessage)
    {
        isGoogleLinkInProgress = false;

        LastGoogleLinkError = success ? "" : errorMessage;

        if (!success)
            Debug.LogError("[Backend] Google account linking failed: " + errorMessage);

        OnChangedLoginGoogle.OnNext(success);
    }

    // Android / iOS 구글 토큰 요청
    private void RequestGoogleToken(bool forceAccountSelection, Action<bool, string, string> callback)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

       if (forceAccountSelection)
{
    TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut(true, (bool isSuccess, string errorMessage) =>
    {
        if (!isSuccess)
        {
            callback(false, "GoogleSignOut failed: " + errorMessage, "");
            return;
        }

        TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(true, (bool success, string error, string token) =>
        {
            callback(success, error, token);
        });
    }); 

    return;
}

TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(true, (bool success, string error, string token) =>
{
    callback(success, error, token);
});

#elif UNITY_IOS && !UNITY_EDITOR

        if (forceAccountSelection)
        {
            TheBackend.ToolKit.GoogleLogin.iOS.GoogleSignOut((bool isSuccess, string errorMessage) =>
            {
                if (!isSuccess)
                {
                    callback(false, "GoogleSignOut failed: " + errorMessage, "");
                    return;
                }

                TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin((bool success, string error, string token) =>
                {
                    callback(success, error, token);
                });
            });

            return;
        }

        TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin((bool success, string error, string token) =>
        {
            callback(success, error, token);
        });

#else

        callback(false, "Google login requires Android or iOS device.", "");

#endif
    }

    // 로그인 실패 처리
    private void FailAuthentication(string errorMessage)
    {
        isAuthRequestInProgress = false;

        IsBackendLoggedIn = false;
        IsGuestAccount = false;

        LastLoginError = errorMessage;

        Debug.LogError("[Backend] " + errorMessage);

        OnBackEndLogin.OnNext(false);
    }

    // 테스트용 게스트 계정 초기화
    [ContextMenu("테스트용 게스트 정보 초기화")]
    public void ResetGuestForTest()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        if (IsBackendLoggedIn || isAuthRequestInProgress)
        {
            Debug.LogWarning("[Backend] Reset guest info only while logged out.");
            return;
        }

        Backend.BMember.DeleteGuestInfo();

        LoginGuest();

#else

        Debug.LogWarning("[Backend] ResetGuestForTest is disabled in release builds.");

#endif
    }

    // 로그인 성공 후 유저 정보 로드
    private void LoginGetUserInfo()
    {
        SendQueue.Enqueue(Backend.BMember.GetUserInfo, bro =>
        {
            if (bro == null || !bro.IsSuccess())
            {
                FailAuthentication("GetUserInfo failed: " + bro);
                return;
            }

            var json = bro.GetReturnValuetoJSON();

            if (json == null || !json.ContainsKey("row") || json["row"] == null)
            {
                FailAuthentication("GetUserInfo response has no row.");
                return;
            }

            JsonData row = json["row"];

            if (row.ContainsKey("nickname") && row["nickname"] != null)
                UserManager.Ins.userNickName = row["nickname"].ToString();

            if (row.ContainsKey("inDate") && row["inDate"] != null)
                UserManager.Ins.userIndate = row["inDate"].ToString();

            if (row.ContainsKey("gamerId") && row["gamerId"] != null)
                UserManager.Ins.userGamerID = row["gamerId"].ToString();

            if (row.ContainsKey("subscriptionType") && row["subscriptionType"] != null)
                IsGuestAccount = row["subscriptionType"].ToString() == "customSignUp";

            IsBackendLoggedIn = true;
            isAuthRequestInProgress = false;

            LastLoginError = "";

            Debug.Log("[Backend] Login verified. IsGuestAccount = " + IsGuestAccount);

            OnBackEndLogin.OnNext(true);
        });
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

                    CacheSnapshot(eGameData);
                    OnGetLoadData.OnNext(eGameData);
                    Debug.Log($"[Backend] Insert Done: {eGameData}");
                });

                return;
            }

            ApplyLoadedData(eGameData, rows[0]);
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
                var data = JsonConvert.DeserializeObject<PuserInfo>(json);
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

        }
    }

    public void SaveData()
    {
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
        }
    }

    private Param GetParamGameData(EGameData eGameData)
    {
        switch (eGameData)
        {
            case EGameData.UserInfo:
                return UserManager.Ins.userInfo.FormatJsonData();

            case EGameData.DungeonInfo:
                return UserManager.Ins.dungeonInfo.FormatJsonData();
            case EGameData.TownInfo:
                return UserManager.Ins.dungeonInfo.FormatJsonData();
            case EGameData.BuildingInfo:
                return UserManager.Ins.dungeonInfo.FormatJsonData();
        }

        Debug.LogError($"[Backend] GetParamGameData Error: {eGameData}");
        return new Param();
    }


    //수정해야하는 함수들 모아놓은 곳
    public void LoadAllChartData()
    {
        bool isSuccess = true;

        isSuccess &= LoadFishChart();
        isSuccess &= LoadItemDataTable();
        isSuccess &= LoadStandChart();
        isSuccess &= LoadEquipmentChart();
        isSuccess &= LoadIngredientChart();
        isSuccess &= LoadStandRecipeChart();
        isSuccess &= LoadStandRecipeInputChart();
        isSuccess &= LoadDisplayStandChart();
        isSuccess &= LoadStoreDataChart();
        OnLoadAllChartData.OnNext(isSuccess);
    }



    //수정해야하는 함수들 모아놓은 곳
    private bool LoadStandRecipeInputChart()
    {
        standRecipeInputDatas.Clear();
        JsonData rows = GetChartRows(standRecipeInputChartFileId, "StandRecipeInputData");
        if (rows == null)
        {
            OnLoadChartItem.OnNext(false);
            return false;
        }
        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            StandRecipeInputData data = new StandRecipeInputData();
            data.recipeId = GetInt(row, "RecipeId");
            data.inputItemId1 = GetInt(row, "InputItemId1");
            data.inputItemId2 = GetInt(row, "InputItemId2");
            data.inputCount = GetInt(row, "InputCount");
            standRecipeInputDatas.Add(data);

        }

        JsonManager.Ins.SetJson<StandRecipeInputData>(standRecipeInputDatas);
        Debug.Log($"[Backend] StandRecipeInput chart load success : {standRecipeInputDatas.Count}");
        return true;
    }

    private bool LoadStandRecipeChart()
    {
        standRecipeDatas.Clear();
        JsonData rows = GetChartRows(standRecipeChartFileId, "StandRecipeData");
        if (rows == null)
        {
            OnLoadChartItem.OnNext(false);
            return false;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            StandRecipeData data = new StandRecipeData();

            data.standId = GetInt(row, "StandId");
            data.recipeId = GetInt(row, "RecipeId");
            data.ProcessTime = GetInt(row, "ProcessTime");
            data.outputItemId = GetInt(row, "OutputItemId");
            data.outputCount = GetInt(row, "OutputCount");
            standRecipeDatas.Add(data);
        }

        JsonManager.Ins.SetJson<StandRecipeData>(standRecipeDatas);
        Debug.Log($"[Backend] StandRecipe chart load success : {standRecipeDatas.Count}");
        return true;
    }

    private bool LoadItemDataTable()
    {
        itemDataTables.Clear();

        JsonData rows = GetChartRows(itemDataTableChartFileId, "ItemDataTable");
        if (rows == null)
        {
            OnLoadChartItem.OnNext(false);
            return false;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            ItemDataTable data = new ItemDataTable();

            data.itemId = GetInt(row, "ItemId");
            data.name = GetString(row, "Name");
            data.itemType1 = GetInt(row, "ItemType1");
            data.itemType2 = GetInt(row, "ItemType2");

            itemDataTables.Add(data);
        }

        JsonManager.Ins.SetJson<ItemDataTable>(itemDataTables);
        OnLoadChartItem.OnNext(true);

        Debug.Log($"[Backend] ItemDataTable chart load success : {itemDataTables.Count}");
        return true;
    }

    private bool LoadFishChart()
    {
        fishDatas.Clear();

        JsonData rows = GetChartRows(fishChartFileId, "Fish");
        if (rows == null)
            return false;

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            FishData data = new FishData();

            data.fishId = GetInt(row, "FishId");
            data.name = GetString(row, "Name");
            data.hp = GetFloat(row, "Hp");
            data.attackDamage = GetFloat(row, "AttackDamage");
            data.attackSpeed = GetFloat(row, "AttackSpeed");
            data.attackRange = GetFloat(row, "AttackRange");
            data.moveSpeed = GetFloat(row, "MoveSpeed");
            data.weight = GetFloat(row, "Weight");
            data.isAttack = GetBool(row, "IsAttack");
            data.detectionRange = GetFloat(row, "DetectionRange");
            data.minDepth = GetFloat(row, "MinDepth");
            data.maxDepth = GetFloat(row, "MaxDepth");

            fishDatas.Add(data);
        }

        JsonManager.Ins.SetJson<FishData>(fishDatas);
        Debug.Log($"[Backend] Fish chart load success : {fishDatas.Count}");
        return true;
    }

    private bool LoadStandChart()
    {
        standDatas.Clear();

        JsonData rows = GetChartRows(standChartFileId, "Stand");
        if (rows == null)
            return false;

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            StandData data = new StandData();

            data.standId = GetInt(row, "StandId");
            data.name = GetString(row, "Name");
            data.standType1 = GetInt(row, "StandType1");
            data.standType2 = GetInt(row, "StandType2");
            data.cost = GetInt(row, "Cost");
            data.stage = GetInt(row, "Stage");

            standDatas.Add(data);
        }

        JsonManager.Ins.SetJson<StandData>(standDatas);
        Debug.Log($"[Backend] Stand chart load success : {standDatas.Count}");
        return true;
    }

    private bool LoadEquipmentChart()
    {
        equipmentDatas.Clear();

        JsonData rows = GetChartRows(equipmentChartFileId, "Equipment");
        if (rows == null)
            return false;

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            EquipmentData data = new EquipmentData();

            data.equipmentId = GetInt(row, "equipmentId");
            data.name = GetString(row, "Name");
            data.equipmentType = GetString(row, "EquipmentType");

            equipmentDatas.Add(data);
        }

        JsonManager.Ins.SetJson<EquipmentData>(equipmentDatas);
        Debug.Log($"[Backend] Equipment chart load success : {equipmentDatas.Count}");
        return true;
    }

    private bool LoadIngredientChart()
    {
        ingredientDatas.Clear();

        JsonData rows = GetChartRows(ingredientChartFileId, "Ingredient");
        if (rows == null)
            return false;

        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            IngredientData data = new IngredientData();

            data.ingredientId = GetInt(row, "IngredientId");
            data.name = GetString(row, "Name");
            data.cost = GetInt(row, "Cost");

            ingredientDatas.Add(data);
        }

        JsonManager.Ins.SetJson<IngredientData>(ingredientDatas);
        Debug.Log($"[Backend] Ingredient chart load success : {ingredientDatas.Count}");
        return true;
    }

    bool LoadDisplayStandChart()
    {
        displayStandDatas.Clear();
        JsonData rows = GetChartRows(displayStandChartFileId, "DisplayStand");
        if (rows == null)
            return false;
        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            DisplayStandData data = new DisplayStandData();
            data.dpStandId = GetInt(row, "DPStandId");
            data.standId = GetInt(row, "StandId");
            data.grade = GetInt(row, "Grade");
            data.stackCount = GetInt(row, "StackCount");
            data.sellingitem = GetInt(row, "SellingItem");
            displayStandDatas.Add(data);
        }
        JsonManager.Ins.SetJson<DisplayStandData>(displayStandDatas);
        Debug.Log($"[Backend] DisplayStand chart load success : {displayStandDatas.Count}");
        return true;
    }

    bool LoadStoreDataChart()
    {
        storeDatas.Clear();
        JsonData rows = GetChartRows(storeDataId, "storeDatas");
        if (rows == null)
            return false;
        for (int i = 0; i < rows.Count; i++)
        {
            JsonData row = rows[i];
            StoreData data = new StoreData();
            data.storeId = GetInt(row, "storeId");
            data.storeType = GetInt(row, "storeType");
            data.targetId = GetInt(row, "targetId");
            data.sellCost = GetInt(row, "sellCost");
            storeDatas.Add(data);
        }
        JsonManager.Ins.SetJson<StoreData>(storeDatas);
        Debug.Log($"[Backend] DisplayStand chart load success : {storeDatas.Count}");
        return true;
    }

    private JsonData GetChartRows(string chartFileId, string chartName)
    {
        if (string.IsNullOrEmpty(chartFileId))
        {
            Debug.LogError($"[Backend] {chartName} chart file id is empty");
            return null;
        }

        BackendReturnObject bro = Backend.Chart.GetChartContents(chartFileId);

        if (bro == null)
        {
            Debug.LogError($"[Backend] {chartName} chart bro is null");
            return null;
        }

        if (!bro.IsSuccess())
        {
            Debug.LogError($"[Backend] {chartName} chart load fail : {bro}");
            return null;
        }

        JsonData rows = bro.FlattenRows();
        if (rows == null)
        {
            Debug.LogError($"[Backend] {chartName} chart rows is null");
            return null;
        }

        return rows;
    }

    private string GetString(JsonData row, string key, string defaultValue = "")
    {
        if (row == null || !row.ContainsKey(key) || row[key] == null)
            return defaultValue;

        return row[key].ToString();
    }

    private int GetInt(JsonData row, string key, int defaultValue = 0)
    {
        if (row == null || !row.ContainsKey(key) || row[key] == null)
            return defaultValue;

        int value;
        if (int.TryParse(row[key].ToString(), out value))
            return value;

        float floatValue;
        if (float.TryParse(row[key].ToString(), out floatValue))
            return Mathf.RoundToInt(floatValue);

        return defaultValue;
    }

    private float GetFloat(JsonData row, string key, float defaultValue = 0f)
    {
        if (row == null || !row.ContainsKey(key) || row[key] == null)
            return defaultValue;

        float value;
        if (float.TryParse(row[key].ToString(), out value))
            return value;

        return defaultValue;
    }

    private bool GetBool(JsonData row, string key, bool defaultValue = false)
    {
        if (row == null || !row.ContainsKey(key) || row[key] == null)
            return defaultValue;

        bool value;
        if (bool.TryParse(row[key].ToString(), out value))
            return value;

        int intValue;
        if (int.TryParse(row[key].ToString(), out intValue))
            return intValue != 0;

        return defaultValue;
    }
}