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
    public float detectionRange;
    public float minDepth;
    public float maxDepth;
}


[Serializable]
public class StandData
{
    public int standId;
    public string name;
    public string standType1;
    public string standType2;
    public int cost;
    public int stage;
}


[Serializable]
public class  ItemDataTable
{
    public int itemId;
    public string name;
    public int itemType1;
    public int itemType2;
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
    public int recipeId;
    public float ProcessTime;
    public int outputItemId;
    public int outputCount;
}

//레시피 인풋을 따로 뺀 이유는 재료의 확장성때문 재료가 n개로 늘어날 수 있다고 생각하기에 따로 뺌
[Serializable]
public class  StandRecipeInputData
{
    public int recipeId;
    public int inputItemId1;
    public int inputItemId2;
    public int inputCount;
}

[Serializable]
public class DisplayStandData
{
    public int dpStandId;
    public int standId;
    public int grade;
    public int stackCount;
    public int sellingitem;
}