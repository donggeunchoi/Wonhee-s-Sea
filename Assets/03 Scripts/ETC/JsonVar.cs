using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FishData
{
    public int id;
    public string name;
    public float hp;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed;
    public float weight;
    public bool isAttack;
}

[Serializable]
public class SashimiData
{
    public int id;
    public string name;
    public int cost;
    public float cookingTime;
    public int cookingStand;
    public int dPStand;
}

[Serializable]
public class SushiData
{
    public int id;
    public string name;
    public int cost;
    public float cookingTime;
    public int cookingStand;
    public int dPStand;
}

[Serializable]
public class DpStandData
{
    public int id;
    public int standType;
    public string standName;
}

[Serializable]
public class ItemChart
{
    public string itemId;
    public string name;
    public string itemType;
    public int itemPower;
    public string itemInfo;

}

[Serializable]
public class EquipmentData
{
    public string itemId;
    public string name;
    public string equipType;
}