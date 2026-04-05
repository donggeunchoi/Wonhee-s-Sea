using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemRuntimeData
{
    public string itemId;
    public string name;
    public EItemType itemType;
    public EIngredientType ingredientType;

    public bool stackable;
    public int maxStack;
}

public class ItemRuntimeDatabase : MonoBehaviour
{
    private Dictionary<string, ItemRuntimeData> itemDataDic = new Dictionary<string, ItemRuntimeData>();

    public bool IsLoaded => itemDataDic != null && itemDataDic.Count > 0;


    public void SetItems(List<ItemDataTable> items)
    {
        itemDataDic.Clear();

        if (items == null)
            return;

        foreach (var table in items)
        {
            if (table == null)
                continue;

            ItemRuntimeData runtimeData = ConvertToRuntimeData(table);
            if (runtimeData == null)
                continue;


            
            itemDataDic[runtimeData.itemId] = runtimeData;
        }

        Debug.Log($"ItemRuntimeDatabase: {itemDataDic.Count}개 로드 완료");
    }

    private ItemRuntimeData ConvertToRuntimeData(ItemDataTable table)
    {
        if (table == null)
            return null;

        ItemRuntimeData data = new ItemRuntimeData();
        data.itemId = table.itemId.ToString();
        data.name = table.name;
        data.itemType = (EItemType)table.itemType1;

        if (data.itemType == EItemType.Ingredient)
            data.ingredientType = (EIngredientType)table.itemType2;
        else
            data.ingredientType = 0;

        data.stackable = true;
        data.maxStack = UserManager.Ins.userInfo.maxCarryCount;

        return data;
    }

    public ItemRuntimeData Get(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        if (itemDataDic == null)
            return null;

        if (itemDataDic.TryGetValue(itemId, out ItemRuntimeData data))
            return data;

        Debug.LogWarning($"ItemRuntimeDatabase: itemId를 찾지 못함. itemId={itemId}");
        return null;
    }

    public bool IsStackable(string itemId)
    {
        return Get(itemId)?.stackable ?? true;
    }

    public int GetMaxStack(string itemId)
    {
        return Get(itemId)?.maxStack ?? 1;
    }
}
