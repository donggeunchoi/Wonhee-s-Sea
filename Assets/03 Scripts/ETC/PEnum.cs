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