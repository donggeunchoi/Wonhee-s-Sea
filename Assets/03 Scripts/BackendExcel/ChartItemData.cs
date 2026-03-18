using System;
using UnityEngine;

[Serializable]
public class ChartItemData
{
    public string itemId;
    public string displayName;
    public int category;
    public bool stackable;
    public int maxStack;
    public string iconKey;
    public string prefabKey;
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(itemId);
    }
}