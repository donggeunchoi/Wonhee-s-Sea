using System;
using System.Collections.Generic;
using UnityEngine;

public class FishResult : MonoBehaviour
{
    public void DungeonFishResult(IEnumerable<int> fishList)
    {
        foreach (int fishID in fishList)
        {
            UserManager.Ins.OnFishID.OnNext(fishID);
        }
    }
}
