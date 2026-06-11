using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemAssetManager : MonoBehaviour
{
    // 아이템 아이디로 아이콘과 프리펩을 조회하는 역할
    [Header("리소시스 아이콘 내부 경로")]
    [SerializeField]
    string iconPath = "Art";

    [Header("리소시스 프리펩 내부 경로")]
    [SerializeField]
    string prefabPath = "Prefabs";

    [SerializeField]
    ItemRuntimeDatabase itemDatabase;

    private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();


    private void Awake()
    {
        if (itemDatabase == null)
            itemDatabase = FindObjectOfType<ItemRuntimeDatabase>();
    }


    public Sprite GetIcon(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("ItemAssetManager: itemId가 null 또는 비어있음");
            return null;
        }

        if (iconCache.TryGetValue(itemId, out Sprite cachedIcon))
            return cachedIcon;

        if (itemDatabase == null)
        {
            Debug.LogError($"ItemAssetManager: itemDatabase가 null임. itemId={itemId}", this);
            return null;
        }

        ItemRuntimeData itemData = itemDatabase.Get(itemId);
        if (itemData == null)
        {
            Debug.LogError($"ItemAssetManager: 아이콘용 itemData를 찾지 못함. itemId={itemId}", this);
            return null;
        }

        string path = GetIconPath(itemData.itemType, itemData.ingredientType, itemId);
        if (string.IsNullOrEmpty(path))
            return null;

        Sprite icon = Resources.Load<Sprite>(path);
        if (icon == null)
        {
            Debug.LogError($"ItemAssetManager: Resources에서 아이콘을 찾을 수 없음. path={path}", this);
            return null;
        }

        iconCache[itemId] = icon;
        return icon;
    }

    public GameObject GetPrefab(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("ItemAssetManager: itemId가 null 또는 비어있음");
            return null;
        }

        if (prefabCache.TryGetValue(itemId, out GameObject cachedPrefab))
            return cachedPrefab;

        if (itemDatabase == null)
        {
            Debug.LogError($"ItemAssetManager: itemDatabase가 null임. itemId={itemId}", this);
            return null;
        }

        ItemRuntimeData itemData = itemDatabase.Get(itemId);
        if (itemData == null)
        {
            Debug.LogError($"ItemAssetManager: 프리팹용 itemData를 찾지 못함. itemId={itemId}", this);
            return null;
        }

        string path = GetPrefabPath(itemData.itemType, itemData.ingredientType, itemId);
        if (string.IsNullOrEmpty(path))
            return null;

        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"ItemAssetManager: Resources에서 프리팹을 찾을 수 없음. path={path}", this);
            return null;
        }

        prefabCache[itemId] = prefab;
        return prefab;
    }

    private string GetPrefabPath(EItemType itemType, EIngredientType ingredientType, string itemId)
    {
        switch (itemType)
        {
            case EItemType.Fish:
                return $"{prefabPath}/Fish/FS{itemId}";

            case EItemType.Ingredient:
                switch (ingredientType)
                {
                    case EIngredientType.Sashimi:
                        return $"{prefabPath}/Sashimi/SSM{itemId}";
                    case EIngredientType.Sushi:
                        return $"{prefabPath}/Sushi/SS{itemId}";
                    case EIngredientType.Evenrice:
                        return $"{prefabPath}/ER{itemId}";
                    case EIngredientType.Rawrice:
                        return $"{prefabPath}/RR{itemId}";
                    default:
                        Debug.LogError($"ItemAssetManager: 잘못된 IngredientType. itemId={itemId}, ingredientType={ingredientType}");
                        return null;
                }

            default:
                Debug.LogError($"ItemAssetManager: 잘못된 ItemType. itemId={itemId}, itemType={itemType}");
                return null;
        }
    }
    private string GetIconPath(EItemType itemType, EIngredientType ingredientType, string itemId)
    {
        switch (itemType)
        {
            case EItemType.Fish:
                return $"{iconPath}/Fish/FS{itemId}";

            case EItemType.Ingredient:
                switch (ingredientType)
                {
                    case EIngredientType.Sashimi:
                        return $"{iconPath}/Sashimi/SSM{itemId}";
                    case EIngredientType.Sushi:
                        return $"{iconPath}/Sushi/SS{itemId}";
                    case EIngredientType.Evenrice:
                        return $"{iconPath}/Evenrice/ER{itemId}";
                    case EIngredientType.Rawrice:
                        return $"{iconPath}/Rawrice/RR{itemId}";
                    default:
                        Debug.LogError($"ItemAssetManager: 잘못된 IngredientType(Icon). itemId={itemId}, ingredientType={ingredientType}", this);
                        return null;
                }

            default:
                Debug.LogError($"ItemAssetManager: 잘못된 ItemType(Icon). itemId={itemId}, itemType={itemType}", this);
                return null;
        }
    }

    public void ClearCache()
    {
        prefabCache.Clear();
    }
}
