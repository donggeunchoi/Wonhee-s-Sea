using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FishDataDef
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
public class SashimiDataDef
{
    public int id;
    public string name;
    public int cost;
    public float cookingTime;
    public int cookingStand;
    public int dPStand;
}

[Serializable]
public class SushiDataDef
{
    public int id;
    public string name;
    public int cost;
    public float cookingTime;
    public int cookingStand;
    public int dPStand;
}

[Serializable]
public class DpStandDataDef
{
    public int id;
    public int standType;
    public string standName;
}

[Serializable]
public class ItemChartDef
{
    public int itemId;
    public string name;
    public string itemType;
    public int itemPower;
    public string itemInfo;

}