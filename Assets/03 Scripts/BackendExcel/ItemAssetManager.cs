using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class ItemAssetEntrt
{
    public string itemId;
    public Sprite Icon;
    public GameObject prefab;
}

public class ItemAssetManager : MonoBehaviour
{
    [SerializeField]
    List<ItemAssetEntrt> items = new List<ItemAssetEntrt>();

    Dictionary<string, ItemAssetEntrt> itemMap = new Dictionary<string, ItemAssetEntrt>();

    private void Awake()
    {
        Build();
    }


    private void Build()
    {
        itemMap.Clear();
        foreach (var entry in items)
        {
            if (entry == null || string.IsNullOrEmpty(entry.itemId))
                continue;
            itemMap[entry.itemId] = entry;
        }
        Debug.Log($"ItemAssetManager: {itemMap.Count}개 아이템 에셋 로드 완료");
    }

    public Sprite GetIcon(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;
        itemMap.TryGetValue(itemId, out var entry);
        return entry?.Icon;
    }

    public GameObject GetPrefab(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;
        return map.TryGetValue(itemId, out var data) ? data.prefab : null;
    }
}
