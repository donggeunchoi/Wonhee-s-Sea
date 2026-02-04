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