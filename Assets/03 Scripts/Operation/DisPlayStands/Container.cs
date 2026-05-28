using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.XR;

public class Container : MonoBehaviour
{
    [Header("표시 루트")]
    [SerializeField] 
    Transform worldStackRoot;

    [Header("보관 옵션")]
    [SerializeField] 
    int maxCapacity;

    [Header("아이콘 조회")]
    [SerializeField] 
    ItemAssetManager itemAssetManager;

    [Header("UI프리펩")]
    [SerializeField]
    FishSlotUI fishSlotUI;
    [Header("UI생성위치")]
    [SerializeField]
    Transform content;

    [Header("캔버스 오브젝트")]
    [SerializeField]
    GameObject visualRoot;

    [Header("플레이어 인벤토리")]
    [SerializeField] 
    Inventory playerInventory;

    [SerializeField] private ItemRuntimeDatabase itemRuntimeDatabase;

    List<FishSlotUI> fishSlotUIs = new List<FishSlotUI>();
    string currentFishId;
    int currentFishCount = 0;
    readonly List<GameObject> pooled = new List<GameObject>();
    GameObject cachedPrefab;


    // Start is called before the first frame update
    private void Awake()
    {
        //구독해놓고 던전에서 넘겨주면 받아서 표출하기.
        BackendManager.Ins.OnTossFish.AsObservable().Subscribe(_toss =>
        {
            if (_toss == null)
                return;

            if (string.IsNullOrEmpty(_toss))
                return;

            InitData(_toss);

        }).AddTo(this);

    }
    private void Start()
    {
        var items = JsonManager.Ins.GetJson<ItemDataTable>();

        if (items == null || items.Count == 0)
        {
            Debug.LogError("ItemDataTable 데이터 없음");
            return;
        }

        itemRuntimeDatabase.SetItems(items);

        TestInit();
    }

    void TestInit()
    {
        List<string> testIds = new List<string>() { "10000", "10002", "10003" };

        for (int i = 0; i < fishSlotUIs.Count; i++)
        {
            if (fishSlotUIs[i] != null)
                Destroy(fishSlotUIs[i].gameObject);
        }

        for (int i = 0; i < testIds.Count; i++)
        {
            int fishCount = 5;
            UserManager.Ins.userInfo.AddFish(testIds[i], fishCount);

            string fishId = testIds[i];

            Sprite fishSprite = itemAssetManager.GetIcon(fishId);
            if (fishSprite == null)
            {
                Debug.LogError($"아이콘 로드 실패 fishId={fishId}");
            }
            FishSlotUI slot = Instantiate(fishSlotUI, content);
            slot.FirstInit(fishSprite, fishId, fishCount, ReceiveFish);
            slot.gameObject.SetActive(true);

            fishSlotUIs.Add(slot);
        }

    }

    public void InitData(string _fishID)
    {
        //던전에서 넘겨주는 데이터로 초기화하기.
        currentFishId = _fishID;
        currentFishCount = UserManager.Ins.userInfo.GetFishCount(currentFishId);

        currentFishCount = Mathf.Clamp(currentFishCount, 0, maxCapacity);

        Sprite fishSprite = itemAssetManager.GetIcon(currentFishId);

        for (int i = 0; i < fishSlotUIs.Count; i++)
        {
            fishSlotUIs[i].FirstInit(fishSprite, currentFishId, currentFishCount, ReceiveFish);
        }
    }

    public void Interact()
    {
        SetVisualActive(true);
    }

    public void SetVisualActive(bool active)
    {
        if (visualRoot == null)
            return;

        visualRoot.SetActive(active);
    }

    void ReceiveFish(string _id) 
    {
        if (string.IsNullOrEmpty(_id))
        {
            Debug.LogWarning("물고기 ID가 비어있음");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogWarning("Player Inventory가 연결되지 않음");
            return;
        }

        int fishCount = UserManager.Ins.userInfo.GetFishCount(_id);

        if (fishCount <= 0)
        {
            Debug.LogWarning($"받을 물고기가 없음 id={_id}");
            return;
        }

        bool success = playerInventory.TryAdd(_id, 1);

        if (!success)
        {
            Debug.LogWarning($"인벤토리에 추가 실패 id={_id}");
            return;
        }

        UserManager.Ins.userInfo.RemoveFish(_id, 1);

        RefreshSlot(_id);

        Debug.LogWarning($"물고기 받기 완료 id={_id}");
    }

    void RefreshSlot(string fishId)
    {
        int fishCount = UserManager.Ins.userInfo.GetFishCount(fishId);
        fishCount = Mathf.Clamp(fishCount, 0, maxCapacity);

        Sprite fishSprite = itemAssetManager.GetIcon(fishId);

        for (int i = 0; i < fishSlotUIs.Count; i++)
        {
            if (fishSlotUIs[i] == null)
                continue;

            if (fishSlotUIs[i].ItemId == fishId)
            {
                fishSlotUIs[i].FirstInit(fishSprite, fishId, fishCount, ReceiveFish);
                break;
            }
        }
    }
} 
