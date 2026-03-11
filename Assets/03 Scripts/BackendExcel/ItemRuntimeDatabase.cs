using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRuntimeDatabase : MonoBehaviour
{
    private Dictionary<string, ChartItemData> itemMap = new Dictionary<string, ChartItemData>();

    public bool IsLoaded => itemMap != null && itemMap.Count > 0;

    public void SetItemS(List<ChartItemData> items)
    {
        itemMap.Clear();

        if (items == null)
            return;

        foreach (var item in items)
        {
            if (item == null || !item.IsValid())
                continue;

            itemMap[item.itemId] = item;
        }

        Debug.Log($"ItemRuntimeDatabase: {itemMap.Count}개 로드 완료");
    }

    public ChartItemData Get(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        itemMap.TryGetValue(itemId, out var data);

        return data;
    }

    public bool IsStackable(string itemId)
    {
        return Get(itemId)?.stackable ?? true;
    }

    public int GetMaxStack(string itemId)
    {
        return Get(itemId)?.maxStack ?? 99;
    }

    public string GetDisplayName(string itemId)
    {
        return Get(itemId)?.displayName ?? itemId;
    }
}
