using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageStand : MonoBehaviour, StandManager.IStand
{
    [Header("진열 아이템 ID (Inventory와 동일)")]
    [SerializeField] private string sushiItemId;

    [Header("보관 용량")]
    [SerializeField] private int maxCapacity = 20;

    [Header("출력 표시 - A(월드 오브젝트 쌓기)")]
    [SerializeField] private bool useWorldStack;
    [SerializeField] private Transform worldStackRoot;
    [SerializeField] private float worldStackOffsetY;
    [SerializeField] private float worldStackOffsetX;
    [SerializeField] private int worldMaxVisible; // 실제로 쌓아두는 오브젝트는 제한(성능)

    [Header("아이템 프리팹 조회용")]
    [SerializeField] private ItemAssetManager itemAssetManager;

    [Header("스택 프리팹(표시용)")]
    [SerializeField] private int current;
    [SerializeField] private TextMeshProUGUI countText;

    private int COLUMN = 5;
    private GameObject cachedPrefab;

    private readonly List<GameObject> pooled = new List<GameObject>();

    public string SellItemId => sushiItemId;
    public int Current => current;

    public int FreeSpace => Mathf.Max(0, maxCapacity - current);

    public StandManager.StandType Type => StandManager.StandType.Sell;
    public bool IsBusy { get; private set; }
    public float progress => 0f;


    private void Start()
    {
        RefreshAll();
    }

    private void OnEnable()
    {
        // 씬에 StandManager가 존재한다면 자신을 등록
        if (StandManager.Ins != null)
        {
            StandManager.Ins.Register(this);
        }
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


        if (string.IsNullOrEmpty(sushiItemId))
            return;

        int space = FreeSpace;

        if (space <= 0)
            return;

        int have = inv.CountOf(sushiItemId);

        if (have <= 0)
            return;

        int toDeposit = Mathf.Min(have, space);

        if (!inv.TryRemove(sushiItemId, toDeposit))
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

        if (string.IsNullOrEmpty(sushiItemId))
        {
            Debug.LogWarning("[StorageStand] sushiItemId가 비어있음", this);
            SetPoolActiveCount(0);
            return;
        }

        var prefab = itemAssetManager.GetPrefab(sushiItemId);
        if (prefab == null)
        {
            Debug.LogWarning($"[StorageStand] prefab 없음. itemId={sushiItemId}", this);
            SetPoolActiveCount(0);
            return;
        }

        if (cachedPrefab != prefab)
        {
            ClearPool();
            cachedPrefab = prefab;
        }

        int visible = Mathf.Clamp(current, 0, worldMaxVisible);

        EnsurePool(prefab, visible);
        SetPoolActiveCount(visible);
        LayoutPoolGrid(visible);
    }


    private void EnsurePool(GameObject prefab, int needCount)
    {
        if (pooled.Count >= needCount)
            return;

        int toCreate = needCount - pooled.Count;
        for (int i = 0; i < toCreate; i++)
        {
            GameObject go = Instantiate(prefab, worldStackRoot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);
            pooled.Add(go);
        }
    }

    private void SetPoolActiveCount(int activeCount)
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            bool active = i < activeCount;
            if (pooled[i] != null && pooled[i].activeSelf != active)
                pooled[i].SetActive(active);
        }
    }

    private void LayoutPoolGrid(int visibleCount)
    {
        if (COLUMN <= 0)
            COLUMN = 5;

        for (int i = 0; i < visibleCount; i++)
        {
            var go = pooled[i];
            if (go == null)
                continue;

            int col = i % COLUMN;
            int row = i / COLUMN;

            go.transform.localPosition = new Vector3(
                col * worldStackOffsetX,
                row * worldStackOffsetY,
                0f
            );

            go.transform.localRotation = Quaternion.identity;
        }
    }

    private void ClearPool()
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] != null)
                Destroy(pooled[i]);
        }

        pooled.Clear();
    }
}
