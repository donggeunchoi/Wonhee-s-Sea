using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StorageStand : MonoBehaviour, StandManager.IStand
{
    [Header("스텐드 ID")]
    [SerializeField]
    int standID;

    [Header("보관 용량")]
    [SerializeField] private int maxCapacity = 20;

    [Header("출력 표시 - A(월드 오브젝트 쌓기)")]
    [SerializeField] private bool useWorldStack;
    [SerializeField] private int worldMaxVisible; // 실제로 쌓아두는 오브젝트는 제한(성능)

    [Header("아이템 프리팹 조회용")]
    [SerializeField] private ItemAssetManager itemAssetManager;

    [Header("스택 프리팹(표시용)")]
    [SerializeField] private int current;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("재료 이미지")]
    [SerializeField]
    Image imgItem;
    [SerializeField]
    Transform worldStackRoot;

    [Header("손님 대기 위치")]
    [SerializeField] private Transform customerWaitPoint;

    [Header("손님 대기열")]
    [SerializeField] private List<CustomerNPC> lineWaiters = new List<CustomerNPC>();

    [SerializeField] private float stepDistance = 0.8f;

    [SerializeField] private Vector3 queueDirection = Vector3.down;

    public Transform CustomerWaitPoint => customerWaitPoint;

    private int COLUMN = 5;
    private Sprite cachedPrefab;

    private readonly List<Image> pooled = new List<Image>();

    public string SellItemId
    {
        get
        {
            if (displayStandData == null || displayStandData.sellingitem <= 0)
                return string.Empty;

            return displayStandData.sellingitem.ToString();
        }
    }

    public int FreeSpace => Mathf.Max(0, maxCapacity - current);

    public StandManager.StandType Type => StandManager.StandType.Sell;
    public bool IsBusy { get; private set; }
    public float progress => 0f;


    List<DisplayStandData> displayStandDatas = new List<DisplayStandData>();
    DisplayStandData displayStandData = null;
    private void Start()
    {
        displayStandDatas = JsonManager.Ins.GetJson<DisplayStandData>();
        displayStandData = displayStandDatas.Find(x => x.standId == standID);

        // 씬에 StandManager가 존재한다면 자신을 등록
        // 매대 등록
        if (StandManager.Ins != null)
        {
            StandManager.Ins.Register(this);

            Debug.Log(
                $"[StorageStand] 매대 등록 완료: " +
                $"StandID={standID}, SellItemID={SellItemId}"
            );
        }
        else
        {
            Debug.LogError("[StorageStand] StandManager가 존재하지 않습니다.");
        }

        RefreshAll();
    }

    private void OnEnable()
    {

    }

    public bool HasStock(int amount = 1)
    {
        if (amount <= 0) return true;
        return current >= amount;
    }

    // 손님이 재고를 집어감
    public bool TryTake(int amount = 1)
    {
        if (amount <= 0) return true;
        if (current < amount) return false;

        current -= amount;
        RefreshAll();
        return true;
    }

    // (선택) 생산/보충용
    public bool TryAddStock(int amount)
    {
        if (amount <= 0) return true;

        int space = FreeSpace;
        if (space <= 0) return false;

        int add = Mathf.Min(space, amount);
        current += add;
        RefreshAll();
        return add == amount;
    }

    public void Interact(Inventory inv)
    {
        if (inv == null)
            return;


        if (displayStandData == null || displayStandData.sellingitem <= 0)
            return;

        int space = FreeSpace;

        if (space <= 0)
            return;

        string sellingItemId = displayStandData.sellingitem.ToString();

        int have = inv.CountOf(sellingItemId);

        if (have <= 0)
            return;

        int toDeposit = Mathf.Min(have, space);

        if (!inv.TryRemove(sellingItemId, toDeposit))
            return;

        current += toDeposit;
        RefreshAll();
    }

    public void Interact(StandManager.StandContext context)
    {
        // 손님, 플레이어 상호작용 로직
    }

    private void RefreshAll()
    {
        RefreshWorldStack();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (countText != null)
            countText.text = $"{current}/{maxCapacity}";
    }

    private void RefreshWorldStack()
    {
        if (!useWorldStack)
            return;

        if (worldStackRoot == null)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (current <= 0)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (itemAssetManager == null)
        {
            Debug.LogWarning("[StorageStand] itemAssetManager가 연결되지 않음", this);
            SetPoolActiveCount(0);
            return;
        }

        if (displayStandData == null || displayStandData.sellingitem <= 0)
        {
            Debug.LogWarning("[StorageStand] sushiItemId가 비어있음", this);
            SetPoolActiveCount(0);
            return;
        }

        string sellingItemId = displayStandData.sellingitem.ToString();

        var prefabsprite = itemAssetManager.GetIcon(sellingItemId);
        if (prefabsprite == null)
        {
            Debug.LogWarning($"[StorageStand] prefab 없음. itemId={displayStandData.standId.ToString()}", this);
            SetPoolActiveCount(0);
            return;
        }

        if (cachedPrefab != prefabsprite)
        {
            ClearPool();
            cachedPrefab = prefabsprite;
        }

        int visible = Mathf.Clamp(current, 0, worldMaxVisible);

        EnsurePool(prefabsprite, visible);
        SetPoolActiveCount(visible);
    }


    private void EnsurePool(Sprite sprite, int needCount)
    {
        if (pooled.Count >= needCount)
            return;

        int toCreate = needCount - pooled.Count;
        for (int i = 0; i < toCreate; i++)
        {
            Image image = Instantiate(imgItem, worldStackRoot);
            image.sprite = sprite;
            image.gameObject.SetActive(false);

            pooled.Add(image);
        }
    }

    private void SetPoolActiveCount(int activeCount)
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] == null)
                continue;

            bool active = i < activeCount;
            pooled[i].gameObject.SetActive(active);
        }
    }

    private void ClearPool()
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] != null)
                Destroy(pooled[i].gameObject);
        }

        pooled.Clear();
    }

    // 손님 대기열
    private Vector3 GetQueuePosition(int index)
    {
        Vector3 startPos = CustomerWaitPoint != null
            ? CustomerWaitPoint.position
            : transform.position;

        return startPos
            + queueDirection.normalized * stepDistance * index;
    }

    // 매대 대기열에 손님 추가
    public Vector3 JoinQueue(CustomerNPC npc)
    {
        if (!lineWaiters.Contains(npc))
        {
            lineWaiters.Add(npc);
        }

        int myIndex = lineWaiters.IndexOf(npc);

        return GetQueuePosition(myIndex);
    }

    // 매대 대기열에서 손님 제거
    public void LeaveQueue(CustomerNPC npc)
    {
        if (lineWaiters.Remove(npc))
        {
            UpdateQueuePositions();
        }
    }

    // 나머지 손님들의 대기 위치 갱신
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < lineWaiters.Count; i++)
        {
            Vector3 newPos = GetQueuePosition(i);

            lineWaiters[i].UpdateStandQueueTarget(newPos);
        }
    }

    // 첫 번째 손님인지 확인
    public bool IsFirstInLine(CustomerNPC npc)
    {
        return lineWaiters.Count > 0
            && lineWaiters[0] == npc;
    }
}
