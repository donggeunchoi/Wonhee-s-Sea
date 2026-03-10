using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageStand : MonoBehaviour
{
    [Header("초밥 아이템 ID (Inventory와 동일)")]
    [SerializeField] private string sushiItemId;

    [Header("보관 용량")]
    [SerializeField] private int maxCapacity = 20;

    [Header("출력 표시 - A(월드 오브젝트 쌓기)")]
    [SerializeField] private bool useWorldStack;
    [SerializeField] private Transform worldStackRoot;
    [SerializeField] private float worldStackOffsetY;
    [SerializeField] private float worldStackOffsetX;
    [SerializeField] private int worldMaxVisible; // 실제로 쌓아두는 오브젝트는 제한(성능)

    [Header("스택 프리팹(표시용)")]
    [SerializeField] private GameObject worldStackPrefab;

    private const int COLUMN = 5;

    private readonly List<GameObject> pooled = new List<GameObject>();

    [SerializeField] private int current;
    [SerializeField] private TextMeshProUGUI countText;

    public string SellItemId => sushiItemId;
    public int Current => current;

    public int FreeSpace => Mathf.Max(0, maxCapacity - current);

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
        RefreshUI();
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
        RefreshUI();
        return add == amount;
    }

    public void Interact(Inventory inv)
    {
        if (inv == null) return;

        int space = FreeSpace;
        if (space <= 0) return;

        int have = inv.CountOf(sushiItemId);
        if (have <= 0) return;

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
            return;
        if (worldStackPrefab == null)
            return;

        int visible = Mathf.Clamp(current, 0, worldMaxVisible);

        EnsurePool(visible);
        
        for(int i = 0; i < pooled.Count; i++ )
        {
            bool active = i < visible;
            if (pooled[i].activeSelf != active)
                pooled[i].SetActive(active);

            if (!active) continue;

            // i번째 아이템의 col/row 계산
            int col = i % COLUMN;     // 0~4
            int row = i / COLUMN;     // 0,1,2...

            // Root 기준으로 회전까지 고려한 배치
            Vector3 pos =
                worldStackRoot.position
                + worldStackRoot.right * (col * worldStackOffsetX)
                + worldStackRoot.up * (row * worldStackOffsetY);

            pooled[i].transform.position = pos;
            pooled[i].transform.rotation = worldStackRoot.rotation;
        }
    }

    private void EnsurePool(int needCount)
    {
        if (pooled.Count >= needCount)
            return;

        int toCreate = needCount - pooled.Count;
        for(int i = 0; i < toCreate; i++)
        {
            GameObject go = Instantiate(worldStackPrefab, worldStackRoot);
            go.SetActive(false);
            pooled.Add(go);
        }
    }

}
