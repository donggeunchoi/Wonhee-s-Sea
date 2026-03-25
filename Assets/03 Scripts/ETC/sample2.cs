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

        InitData2();
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

        Dictionary<int, ItemDataTable> itemDic = JsonManager.Ins.GetJsonDicInt<ItemDataTable>(info => info.ItemId);
        string itemName = itemDic[fishID].Name;

        Debug.Log($"5번 : {itemName}");

        var itemDatas = BackendManager.Ins.itemDataTables;

        var itemdata = itemDatas.Find(info => info.ItemId == fishID);

        Debug.Log($"6번 : {itemdata.Name}");


        var Tests = JsonManager.Ins.GetJson<ItemDataTable>();

        if (Tests == null)
            return;

        var Test = Tests.Find(info => info.ItemId == fishID);

        Debug.LogWarning($"7번 : {Test.Name}");
    }

    private void InitData2()
    {
        var ItemDataJsons = JsonManager.Ins.GetJson<ItemDataTable>();

        if (ItemDataJsons == null)
        {
            Debug.LogWarning($"ItemDataJsons가 없다");
        }
            

        var ItemDataJson = ItemDataJsons.Find(info => info.ItemId == fishID);

        if (ItemDataJson == null)
            return;

        Debug.Log($"4번 : {ItemDataJson.Name}");
    }
}
