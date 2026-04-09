using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class LoginManager : MonoBehaviour
{
    [Header("로그인 버튼들")]
    [SerializeField] private GameObject objAuthGroup;
    [SerializeField] private Button btAuthGoogle;
    [SerializeField] private Button btAuthGuest;

    [Header("로그인 진행 상황")]
    [SerializeField] private Text txLoading;

    [Header("로그인 후 이동할 씬 이름")]
    [SerializeField] private string nextSceneName;

    private bool isInitDone = false;
    private bool isLoginDone = false;
    private bool isChartDataLoaded = false;
    private bool isGameDataLoaded = false;

    private int loadedCount = 0;
    private int totalCount = 0;

    private void Start()
    {
        if (txLoading != null)
            txLoading.text = "";

        if (objAuthGroup != null)
            objAuthGroup.SetActive(false);

        SetLoginButtons();

        BackendManager.Ins.OnInitBackEnd
            .Where(ok => ok)
            .Take(1)
            .Subscribe(_ =>
            {
                isInitDone = true;

                if (txLoading != null)
                    txLoading.text = "초기화 완료. 로그인 해주세요";

                if (objAuthGroup != null)
                    objAuthGroup.SetActive(true);
            }).AddTo(this);

        BackendManager.Ins.OnBackEndLogin
            .Take(1)
            .Subscribe(success =>
            {
                if (!success)
                {
                    Debug.LogError("로그인에 실패했습니다.");

                    if (txLoading != null)
                        txLoading.text = "로그인에 실패했습니다. 다시 시도해주세요.";

                    if (objAuthGroup != null)
                        objAuthGroup.SetActive(true);

                    return;
                }

                isLoginDone = true;

                if (txLoading != null)
                    txLoading.text = "로그인에 성공했습니다. 데이터를 불러오는 중...";

                BackendManager.Ins.LoadAll();
                BackendManager.Ins.LoadAllChartData();
            }).AddTo(this);

        loadedCount = 0;
        totalCount = Enum.GetValues(typeof(EGameData)).Length;

        BackendManager.Ins.OnGetLoadData
            .Subscribe(_ =>
            {
                loadedCount++;

                if (txLoading != null)
                    txLoading.text = $"데이터 불러오는 중... {loadedCount}/{totalCount}";

                if (loadedCount >= totalCount)
                {
                    isGameDataLoaded = true;
                    TryMoveScene();
                }
            }).AddTo(this);

        BackendManager.Ins.OnLoadChartItem
            .Where(x => x)
            .Take(1)
            .Subscribe(_ =>
            {
                isChartDataLoaded = true;
                TryMoveScene();
            }).AddTo(this);

        if (txLoading != null)
            txLoading.text = "백엔드 초기화 중...";

        BackendManager.Ins.InitBackEnd();
    }

    private void TryMoveScene()
    {
        if (!isLoginDone)
            return;

        if (!isGameDataLoaded)
            return;

        if (!isChartDataLoaded)
            return;

        if (txLoading != null)
            txLoading.text = "모든 데이터 로드 완료! 게임 씬으로 이동합니다.";

        SceneManager.LoadScene(nextSceneName);
    }

    public void SetLoginButtons()
    {
        if (btAuthGoogle != null)
        {
            btAuthGoogle.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    Debug.Log("Google Login: 아직 미구현");
                    if (txLoading != null)
                        txLoading.text = "Google 로그인은 아직 미구현입니다.";
                }).AddTo(this);
        }

        if (btAuthGuest != null)
        {
            btAuthGuest.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    if (!isInitDone)
                    {
                        if (txLoading != null)
                            txLoading.text = "아직 초기화 중입니다...";
                        return;
                    }

                    if (objAuthGroup != null)
                        objAuthGroup.SetActive(false);

                    if (txLoading != null)
                        txLoading.text = "게스트 로그인 중...";

                    BackendManager.Ins.LoginGuest();
                }).AddTo(this);
        }
    }
}