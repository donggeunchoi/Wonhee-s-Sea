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


    public int fishID = 10001;

    public int equipID = 50000;

    FishData fishData = null;
   

    private void Start()
    {
        initData();
    }

    private void initData()
    {
        Dictionary<int, FishData> fishDict = JsonManager.Ins.GetJsonDicInt<FishData>(info => info.fishId);

        string name = fishDict[fishID].name;

        Debug.Log($"1번 : {name}");

        FishData fishData = JsonManager.Ins.GetDataByKey<FishData>(fishID, info => info.fishId);

        Debug.Log($"2번 : {fishData.hp}");

        EquipmentData equipmentData = JsonManager.Ins.GetDataByKey<EquipmentData>(equipID, info => info.equipmentId);
        
        Debug.Log($"3번 :{equipmentData.equipmentType}");
    }
}
