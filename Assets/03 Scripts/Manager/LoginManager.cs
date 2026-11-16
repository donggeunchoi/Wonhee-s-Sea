using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("로그인 버튼들")]
    [SerializeField] private GameObject objAuthGroup;
    [SerializeField] private Button btAuthGoogle;
    [SerializeField] private Button btAuthGuest;

    [Header("로그인 진행 상황")]
    [SerializeField] private Text txLoading;
    [SerializeField] private TextMeshProUGUI txLoad;

    [Header("로그인 후 이동할 씬 이름")]
    [SerializeField] private string nextSceneName;

    private bool isInitDone = false;
    private bool isLoginDone = false;
    private bool isChartDataLoaded = false;
    private bool isGameDataLoaded = false;

    private bool isLoginInProgress = false;
    private bool isSceneLoading = false;
    private bool isDataLoadFailed = false;
    private bool isButtonBound = false;

    private int totalCount = 0;

    private readonly HashSet<EGameData> loadedGameData = new HashSet<EGameData>();

    private void Start()
    {
        totalCount = Enum.GetValues(typeof(EGameData)).Length;
        loadedGameData.Clear();

        SetLoadingMessage("백엔드 초기화 중...");
        SetAuthGroupActive(false);

        SetLoginButtons();
        SubscribeBackendEvents();

        BackendManager.Ins.InitBackEnd();
    }

    // 백엔드 이벤트 구독
    private void SubscribeBackendEvents()
    {
        BackendManager.Ins.OnInitBackEnd
            .Subscribe(OnBackendInitialized)
            .AddTo(this);

        BackendManager.Ins.OnBackEndLogin
            .Subscribe(OnBackendLoginResult)
            .AddTo(this);

        BackendManager.Ins.OnGetLoadData
            .Subscribe(OnGameDataLoaded)
            .AddTo(this);

        BackendManager.Ins.OnLoadGameDataFailed.Subscribe( OnGameDataLoadFailed ).AddTo( this );

        BackendManager.Ins.OnLoadAllChartData
            .Subscribe(OnChartLoadResult)
            .AddTo(this);
    }

    // 백엔드 초기화 결과
    private void OnBackendInitialized(bool success)
    {
        if (!success)
        {
            isInitDone = false;

            Debug.LogError("[Login] Backend initialization failed.");
            SetLoadingMessage("서버 초기화에 실패했습니다. 게임을 다시 실행해주세요.");
            SetAuthGroupActive(false);

            return;
        }

        isInitDone = true;

        Debug.Log("[Login] Backend initialization success.");

        SetLoadingMessage( string.Empty );
        SetAuthGroupActive(true);
    }

    // 로그인 버튼 이벤트 등록
    public void SetLoginButtons()
    {
        if (isButtonBound)
            return;

        isButtonBound = true;

        if (btAuthGuest != null)
        {
            btAuthGuest.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => TryLogin(false))
                .AddTo(this);
        }

        if (btAuthGoogle != null)
        {
            btAuthGoogle.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => OnClickGoogleLogin())
                .AddTo(this);
        }
    }

    // 구글 로그인 버튼
    public void OnClickGoogleLogin()
    {
        TryLogin(true);
    }

    // 공통 로그인 요청
    private void TryLogin(bool isGoogle)
    {
        if (!isInitDone)
        {
            SetLoadingMessage("아직 초기화 중입니다...");
            return;
        }

        if (isLoginDone || isLoginInProgress || isSceneLoading)
            return;

        if (isGoogle && Application.isEditor)
        {
            SetLoadingMessage("구글 로그인은 Android 또는 iOS 기기에서 테스트해주세요.");
            return;
        }

        isLoginInProgress = true;

        SetAuthGroupActive(false);

        SetLoadingProgress( isGoogle ? "Google 계정 인증 중..." : "게스트 로그인 중..." );

        try
        {
            if (isGoogle)
                BackendManager.Ins.LoginGoogle();
            else
                BackendManager.Ins.LoginGuest();
        }
        catch (Exception exception)
        {
            isLoginInProgress = false;

            Debug.LogError("[Login] Login exception: " + exception);
            SetLoadingMessage("로그인 실행 오류: " + exception.Message);
            SetAuthGroupActive(true);
        }
    }

    // 공통 로그인 결과
    private void OnBackendLoginResult(bool success)
    {
        if (isLoginDone || isSceneLoading)
            return;

        isLoginInProgress = false;

        if (!success)
        {
            string errorMessage = BackendManager.Ins.LastLoginError;

            Debug.LogError("[Login] Login failed: " + errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = "로그인에 실패했습니다. 다시 시도해주세요.";

            SetLoadingMessage(errorMessage);
            SetAuthGroupActive(true);

            return;
        }

        isLoginDone = true;

        isGameDataLoaded = false;
        isChartDataLoaded = false;
        isDataLoadFailed = false;

        loadedGameData.Clear();

        SetAuthGroupActive(false);
        ReflashDataLoadingProgress();

        Debug.Log("[Login] Authentication success. Starting data loading.");

        try
        {
            BackendManager.Ins.LoadAll();
            BackendManager.Ins.LoadAllChartData();
        }
        catch (Exception exception)
        {
            isDataLoadFailed = true;
            Debug.LogError("[Login] Data loading exception: " + exception);
            SetLoadingMessage("데이터 로딩 실행 중 오류가 발생했습니다.");
        }
    }

    // 게임 데이터 로드 결과
    private void OnGameDataLoaded(EGameData gameData)
    {
        if( !isLoginDone || isSceneLoading || isDataLoadFailed )
            return;

        if (!loadedGameData.Add(gameData))
        {
            Debug.LogWarning("[Login] Duplicate game data response: " + gameData);
            return;
        }

        int loadedCount = loadedGameData.Count;

        Debug.Log($"[Login] Game data loaded: {gameData} ({loadedCount}/{totalCount})");

        isGameDataLoaded = loadedCount == totalCount;
        ReflashDataLoadingProgress();

        if( !isGameDataLoaded )
            return;

        Debug.Log("[Login] All game data loaded.");

        TryMoveScene();
    }

    private void OnGameDataLoadFailed( EGameData gameData )
    {
        if( !isLoginDone || isSceneLoading || isDataLoadFailed )
            return;

        isDataLoadFailed = true;
        SetLoadingMessage( $"플레이어 데이터를 불러오지 못했습니다.\n({gameData}) 게임을 다시 실행해 주세요." );
    }

    // 전체 차트 로드 결과
    private void OnChartLoadResult(bool success)
    {
        if( !isLoginDone || isSceneLoading || isDataLoadFailed )
            return;

        isChartDataLoaded = success;
        if (!success)
        {
            isDataLoadFailed = true;
            Debug.LogError("[Login] Chart loading failed.");
            SetLoadingMessage("차트 로드에 실패했습니다. 오류 로그를 확인해주세요.");
            return;
        }

        Debug.Log("[Login] All chart data loaded.");

        ReflashDataLoadingProgress();
        TryMoveScene();
    }

    private void ReflashDataLoadingProgress()
    {
        if( isGameDataLoaded && isChartDataLoaded )
            return;

        if( !isGameDataLoaded )
        {
            SetLoadingProgress( $"플레이어 데이터 불러오는 중 ({loadedGameData.Count}/{totalCount})" );
            return;
        }

        SetLoadingProgress( "게임 설정 데이터 불러오는 중..." );
    }

    private void SetLoadingProgress( string message )
    {
        const int AUTH_AND_CHART_STEP_COUNT = 2;
        int completedSteps = loadedGameData.Count + ( isLoginDone ? 1 : 0 ) + ( isChartDataLoaded ? 1 : 0 );
        int progress = completedSteps * 100 / ( totalCount + AUTH_AND_CHART_STEP_COUNT );
        SetLoadingMessage( $"{progress}%\n{message}" );
    }

    // 게임 씬 이동 조건 검사
    private void TryMoveScene()
    {
        if (!isLoginDone)
            return;

        if (!isGameDataLoaded)
            return;

        if (!isChartDataLoaded)
            return;

        if (isSceneLoading)
            return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[Login] Next scene name is empty.");
            SetLoadingMessage("이동할 게임 씬이 설정되지 않았습니다.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError("[Login] Scene not found in build: " + nextSceneName);
            SetLoadingMessage("게임 씬을 찾을 수 없습니다. 빌드 설정을 확인해주세요.");
            return;
        }

        isSceneLoading = true;

        SetLoadingProgress( "데이터 로드 완료" );
        Observable.NextFrame().Subscribe( _ => LoadGameScene() ).AddTo( this );
    }

    private void LoadGameScene()
    {
        Debug.Log("[Login] Loading game scene: " + nextSceneName);

        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (Exception exception)
        {
            isSceneLoading = false;

            Debug.LogError("[Login] Scene loading failed: " + exception);
            SetLoadingMessage("게임 씬 이동에 실패했습니다.");
        }
    }

    // 공통 로딩 메시지 출력
    private void SetLoadingMessage(string message)
    {
        if (txLoading != null)
            txLoading.text = message;

        if (txLoad != null)
            txLoad.text = message;
    }

    // 로그인 버튼 그룹 활성화
    private void SetAuthGroupActive(bool active)
    {
        if (objAuthGroup != null)
            objAuthGroup.SetActive(active);
    }
}
