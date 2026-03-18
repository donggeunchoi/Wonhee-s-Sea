using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class fishMan
{
   public int fishid;
}

public class sample2 : MonoBehaviour
{
    List<FishData> fishDatas = new List<FishData>();

    [SerializeField]
    List<fishMan> fishMen = new List<fishMan>();

    FishData fishData = null;
    private void Awake()
    {
        UserManager.Ins.OnFishID.AsObservable().Subscribe(fishId =>
        {
            Debug.Log(fishId);

            var fishDatas = JsonManager.Ins.GetJson<FishData>();

            foreach( var data in fishDatas)
            {
                Debug.Log(data.name);
            }

            var fishData = fishDatas.Find(x => x.fishId == fishId);

            fishDatas.Add(fishData);

            Debug.Log(fishData.name);

        }).AddTo(this);

        
    }

    private void Start()
    {
        foreach (var data in fishMen)
        {
            fishData = JsonManager.Ins.GetJson<FishData>().Find(info => info.fishId == data.fishid);
        }
    }
}
