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

//전체적인 레시피 결과물은 결국 1개일 태니 레시피는 이렇게 관리하는게 효율적일듯
[Serializable]
public class  StandRecipeData
{
    public int standId;
    public string recipeId;
    public float progressTime;
    public string outputItemId;
    public int outputCount;
}

//레시피 인풋을 따로 뺀 이유는 재료의 확장성때문 재료가 n개로 늘어날 수 있다고 생각하기에 따로 뺌
[Serializable]
public class  StandRecipeInputData
{
    public string recipeId;
    public string inputItemId;
    public int inputCount;
}