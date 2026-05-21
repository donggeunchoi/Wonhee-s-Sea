using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StorageStand : MonoBehaviour
{
    [Header("진열 아이템 ID (Inventory와 동일)")]
    [SerializeField] private string sushiItemId;

    public int standID;

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

    private int COLUMN = 5;
    private Sprite cachedPrefab;

    private readonly List<Image> pooled = new List<Image>();

    public string SellItemId => sushiItemId;
    public int Current => current;

    public int FreeSpace => Mathf.Max(0, maxCapacity - current);
    private void Start()
    {
        RefreshAll();
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

        var prefabsprite = itemAssetManager.GetIcon(sushiItemId);
        if (prefabsprite == null)
        {
            Debug.LogWarning($"[StorageStand] prefab 없음. itemId={sushiItemId}", this);
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
}
