using System;

[Serializable]
public class FishData
{
    public int fishId;
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
public class StandData
{
    public int standId;
    public string name;
    public string standType1;
    public string standType2;
    public int ingredient_1;
    public int ingredient_2;
    public float time;
    public int cost;
}

[Serializable]
public class  ItemDataTable
{
    public int ItemId;
    public string Name;
    public string ItemType1;
    public string ItemType2;
}

[Serializable]
public class EquipmentData
{
    public int equipmentId;
    public string name;
    public string equipmentType;
}

[Serializable]
public class IngredientData
{
    public int ingredientId;
    public string name;
    public int cost;
}