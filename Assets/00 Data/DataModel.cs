using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipmentType
{
    Weapon
}

public enum StandType
{
    CuttingBoard,
    RiceCooker,
    CounterTop,
    DisplayStand,
    SalesStand
}



[Serializable]
public class FishRaw
{
    public string fishId;
    public string Name;
    public string Hp;
    public string AttackDamage;
    public string AttackSpeed;
    public string AttackRange;
    public string MoveSpeed;
    public string Weight;
    public string IsAttack;
}

public class FishDataDef
{
    public readonly int fishId;
    public readonly string Name;
    public readonly float Hp;
    public readonly float AttackDamage;
    public readonly float AttackSpeed;
    public readonly float AttackRange;
    public readonly float MoveSpeed;
    public readonly float Weight;
    public readonly bool IsAttack;

    public FishDataDef(FishRaw raw)
    {
        this.fishId = DataParse.ParseSafe<int>(raw.fishId);
        this.Name = DataParse.ParseSafe<string>(raw.Name);
        this.Hp = DataParse.ParseSafe<float>(raw.Hp);
        this.AttackDamage = DataParse.ParseSafe<float>(raw.AttackDamage);
        this.AttackSpeed = DataParse.ParseSafe<float>(raw.AttackSpeed);
        this.AttackRange = DataParse.ParseSafe<float>(raw.AttackRange);
        this.MoveSpeed = DataParse.ParseSafe<float>(raw.MoveSpeed);
        this.Weight = DataParse.ParseSafe<float>(raw.Weight);
        this.IsAttack = DataParse.ParseSafe<bool>(raw.IsAttack);
    }
}



[Serializable]
public class SashimiRaw
{
    public string sashimiId;
    public string Cost;
    public string CookingStand;
    public string DPStand;
}

public class SashimiDataDef
{
    public int sashimiId;
    public float Cost;
    public int CookingStand;
    public int DPStand;

    public SashimiDataDef(SashimiRaw raw)
    {
        this.sashimiId = DataParse.ParseSafe<int>(raw.sashimiId);
        this.Cost = DataParse.ParseSafe<float>(raw.Cost);
        this.CookingStand = DataParse.ParseSafe<int>(raw.CookingStand);
        this.DPStand = DataParse.ParseSafe<int>(raw.DPStand);
    }
}



[Serializable]
public class SushiRaw
{
    public string sushiId;
    public string Cost;
    public string CookingStand;
    public string DPStand;
}

public class SushiDataDef
{
    public int sushiId;
    public float Cost;
    public int CookingStand;
    public int DPStand;

    public SushiDataDef(SushiRaw raw)
    {
        this.sushiId = DataParse.ParseSafe<int>(raw.sushiId);
        this.Cost = DataParse.ParseSafe<float>(raw.Cost);
        this.CookingStand = DataParse.ParseSafe<int>(raw.CookingStand);
        this.DPStand = DataParse.ParseSafe<int>(raw.DPStand);
    }
}



[Serializable]
public class StandRaw
{
    public string standId;
    public string StandType;
    public string Ingredient_1;
    public string Ingredient_2;
    public string Time;
    public string Cost;
}

public class StandDataDef
{
    public int standId;
    public StandType StandType;
    public int Ingredient_1;
    public int Ingredient_2;
    public float Time;
    public float Cost;

    public StandDataDef(StandRaw raw)
    {
        Debug.Log($"[Debug] RawID: {raw.standId}, RawType: {raw.StandType}, RawIngredient_1: {raw.Ingredient_1}, RawIngredient_2: {raw.Ingredient_2}, RawTime: {raw.Time}, RawCost: {raw.Cost}");

        this.standId = DataParse.ParseSafe<int>(raw.standId);
        this.StandType = DataParse.ParseSafe<StandType>(raw.StandType);
        this.Ingredient_1 = DataParse.ParseSafe<int>(raw.Ingredient_1);
        this.Ingredient_2 = DataParse.ParseSafe<int>(raw.Ingredient_2);
        this.Time = DataParse.ParseSafe<float>(raw.Time);
        this.Cost = DataParse.ParseSafe<float>(raw.Cost);
    }
}



[Serializable]
public class EquipmentRaw
{
    public string equipmentId;
    public string Name;
    public string EquipmentType;
}

public class EquipmentDataDef
{
    public int equipmentId;
    public string Name;
    public EquipmentType EquipmentType;

    public EquipmentDataDef(EquipmentRaw raw)
    {
        this.equipmentId = DataParse.ParseSafe<int>(raw.equipmentId);
        this.Name = DataParse.ParseSafe<string>(raw.Name);
        this.EquipmentType = DataParse.ParseSafe<EquipmentType>(raw.EquipmentType);
    }

}

// [Serializable]
// public class ItemChartDef
// {
//     public int itemId;
//     public string name;
//     public string itemType;
//     public int itemPower;
//     public string itemInfo;

// }