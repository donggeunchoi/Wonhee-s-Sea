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

    public int FreeSpace => Mathf.Max(0, maxCapacity - current);

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
        // Debug.Log($"[StorageStand] +{toDeposit} => {current}/{maxCapacity}");
    }

    private void RefreshUI()
    {
        if (countText != null)
            countText.text = $"{current}/{maxCapacity}";
    }

}
