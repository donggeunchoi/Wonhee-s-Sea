using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum EGameData
{
    UserInfo,
    DungeonInfo,
    TownInfo,
    BuildingInfo,
}

[Serializable]
public enum  ENPCState
{
    MoveToStand,
    Pick,
    MoveToCheckout,
    Pay,
    Exit
}

[Serializable]
public enum  EItemType
{
    Fish = 1,
    Ingredient = 2,// 재료 안에서의 티입2는 (1: 회, 2: 초밥, 3:지은밥, 4:생쌀)
}

[Serializable]
public enum EIngredientType
{
    Sashimi = 1,
    Sushi = 2,
    Evenrice = 3,
    Rawrice = 4
}

[Serializable]
public enum  EQuestType
{
    None = 0,
    EnterScene = 1,
    CatchFish = 2,
    ProcessItem = 3,
    HireEmployee = 4,
    UpgradeEmployee = 5,
}

[Serializable]
public enum ETargetType
{
    None = 0,
    Scene = 1,
    Fish = 2,
    Employee = 3,
    Item = 4,
}

//[Serializable]
//public enum  ECollectionVelue
//{
//    None = 0,
//    Fish = 1,// Type2는 FishID
//    Gold = 2,
//    Dia = 3,
//    Rating = 4,// 평판
//    Ingredient = 5, // Type2는 EIngredientType, Type3은 IngredientID
//}


[Serializable]
public enum EStoreType
{
    Equipment = 1,
    Ingredient = 2,
    Employee = 3,
    Skin = 4
}