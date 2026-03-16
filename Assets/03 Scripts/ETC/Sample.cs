using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    int fishid = 5;

    public void InitData()
    {
        UserManager.Ins.OnFishID.OnNext(fishid);
    }

}
