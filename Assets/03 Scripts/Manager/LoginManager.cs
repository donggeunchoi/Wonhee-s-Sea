using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class LoginManager : MonoBehaviour
{
    [Header("로그인 버튼들")]
    [SerializeField]
    private GameObject objAuthGroup;
    [SerializeField]
    private Button btAuthGoogle;
    [SerializeField]
    private Button btAuthGuest;

    [Header("로그인 진행 상황")]
    [SerializeField]
    private Text txLoading;

    [Header("로그인 후 이동할 씬 이름")]
    [SerializeField]
    private string nextSceneName = "Dong_TownScene";

    private bool isInitDone = false;
    private bool isLoginDone = false;

    private void Start()
    {
        if (txLoading != null)
            txLoading.text = "";
        if(objAuthGroup != null)
            objAuthGroup.SetActive(false);

        SetLoginButtons();

        //초기화 완료되면 로그인 UI 활성화하기
        BackendManager.Ins.OnInitBackEnd.Where(ok => ok).Take(1).Subscribe( _=>
        {
            isInitDone = true;
            if( txLoading != null)
                txLoading.text = "초기화 완료. 로그인 해주세요";
            if (objAuthGroup != null)
                objAuthGroup.SetActive(true);
        }).AddTo(this);

        //로그인이 완료되면 모두로드 시작하기
        BackendManager.Ins.OnBackEndLogin.Take(1).Subscribe( success =>
        {
            if( !success)
            {
                Debug.LogError("로그인에 실패했습니다.");
                if (txLoading != null)
                    txLoading.text = "로그인에 실패했습니다. 다시 시도해주세요.";
                if (objAuthGroup != null)
                    objAuthGroup.SetActive(true);
                return;
            }

            isLoginDone= true;
            if(txLoading != null)
                txLoading.text = "로그인에 성공했습니다. 데이터를 불러오는 중...";
            BackendManager.Ins.LoadAll();
            BackendManager.Ins.LoadCharItem();
        }).AddTo(this);


        //하나하나 데이터 로드하는 것 체크하기
        int loadedCount = 0;
        int totalCount = Enum.GetValues(typeof(EGameData)).Length; // 현재는 User, Dungeon, Town, Building

        BackendManager.Ins.OnGetLoadData.Subscribe (_ =>
        {
            loadedCount++;
            if (txLoading != null)
                txLoading.text = $"데이터 불러오는 중... {loadedCount}/{totalCount}";
            if (loadedCount >= totalCount)
            {
                if (txLoading != null)
                    txLoading.text = "모든 데이터 로드 완료! 게임 씬으로 이동합니다.";
                SceneManager.LoadScene(nextSceneName);
            }
        }).AddTo(this);
        
        if(txLoading != null)
            txLoading.text = "백엔드 초기화 중...";
        BackendManager.Ins.InitBackEnd();

    }

    public void SetLoginButtons()
    {
        if (btAuthGoogle != null)
        {
            btAuthGoogle.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(1)) // 연속클릭 방지
                .Subscribe(_ =>
            {
                Debug.Log("Google Login: 아직 미구현");
                if (txLoading != null) txLoading.text = "Google 로그인은 아직 미구현입니다.";
            }).AddTo(this);
        }

        if (btAuthGuest != null)
        {
            btAuthGuest.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(1)) // 연속클릭 방지
                .Subscribe(_ =>
            {
                if (!isInitDone)
                {
                    if (txLoading != null) txLoading.text = "아직 초기화 중입니다...";
                    return;
                }

                if (objAuthGroup != null) objAuthGroup.SetActive(false);
                if (txLoading != null) txLoading.text = "게스트 로그인 중...";

                BackendManager.Ins.LoginGuest();
            }).AddTo(this);
        }
    }
}
