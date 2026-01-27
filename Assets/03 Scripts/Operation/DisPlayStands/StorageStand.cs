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
        RefreshUI();
        
    }

    private void RefreshUI()
    {
        if (countText != null)
            countText.text = $"{current}/{maxCapacity}";
    }

}
