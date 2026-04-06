using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemAssetEntry
{
    public string itemId;
    public Sprite Icon;
    public GameObject prefab;
}

public class ItemAssetManager : MonoBehaviour
{
    [Header("리소시스 아트 내부 경로")]
    [SerializeField]
    string resourcePath = "Art";

    [Header("리소시스 프리펩 내부 경로")]
    [SerializeField]
    string prefabPath = "Prefabs";

    private ItemRuntimeDatabase itemDatabase;

    private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();


    [SerializeField]
    List<ItemAssetEntry> items = new List<ItemAssetEntry>();

    Dictionary<string, ItemAssetEntry> itemMap = new Dictionary<string, ItemAssetEntry>();

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
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("ItemAssetManager: itemId가 null 또는 비어있음");
            return null;
        }

        var itemData = itemDatabase.Get(itemId);

        EItemType itemType = (EItemType)itemData.itemType;
        EIngredientType ingredientType = (EIngredientType)itemData.ingredientType;

        string path = GetPrefabPath(itemType, ingredientType, itemId);
        if (string.IsNullOrEmpty(path))
            return null;

        if (prefabCache.TryGetValue(itemId, out GameObject cachedPrefab))
            return cachedPrefab;

        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"ItemAssetManager: Resources에서 프리팹을 찾을 수 없음. path={path}");
            return null;
        }

        prefabCache[path] = prefab;
        return prefab;
    }

    private string GetPrefabPath(EItemType itemType, EIngredientType ingredientType, string itemId)
    {
        switch (itemType)
        {
            case EItemType.Fish:
                return $"{prefabPath}/Fish/{itemId}";

            case EItemType.Ingredient:
                switch (ingredientType)
                {
                    case EIngredientType.Sashimi:
                        return $"{prefabPath}/Sashimi/{itemId}";
                    case EIngredientType.Sushi:
                        return $"{prefabPath}/Sushi/{itemId}";
                    case EIngredientType.Evenrice:
                        return $"{prefabPath}/Evenrice/{itemId}";
                    case EIngredientType.Rawrice:
                        return $"{prefabPath}/Rawrice/{itemId}";
                    default:
                        Debug.LogError($"ItemAssetManager: 잘못된 IngredientType. itemId={itemId}, ingredientType={ingredientType}");
                        return null;
                }

            default:
                Debug.LogError($"ItemAssetManager: 잘못된 ItemType. itemId={itemId}, itemType={itemType}");
                return null;
        }
    }

    public void ClearCache()
    {
        prefabCache.Clear();
    }
}
