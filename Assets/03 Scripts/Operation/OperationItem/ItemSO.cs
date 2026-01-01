using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "GameData/Item")]
public class ItemSO : ScriptableObject
{
    [Header("아이템 고유 번호 및 이름")]
    public string id;
    public string disPlayName;

    [Header("UI")]
    public Sprite icon;

    [Header("stack")]
    public bool stackable = true;
}
