using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    Material,
    Product
}

[CreateAssetMenu(fileName = "Item_", menuName = "GameData/Item")]
public class ItemSO : ScriptableObject
{
    public string itemId;
    public string displayName;
    public ItemCategory Category;
    public Sprite icon;
    public bool stackable = true;
    public int maxStack = 90;
    public GameObject prefab;
}
