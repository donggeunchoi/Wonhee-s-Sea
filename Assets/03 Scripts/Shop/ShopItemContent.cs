using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemContent : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField]
    Image imgIcon;

    [Header("이름")]
    [SerializeField]
    TextMeshProUGUI txName;

    [Header("가격")]
    [SerializeField]
    TextMeshProUGUI txCost;

    public void InitData(StoreData _storeData)
    {
        //imgIcon.sprite = ItemAssetManager.Ins.GetIcon(_storeData.targetId.ToString());

        var itemTable = JsonManager.Ins.GetJson<ItemDataTable>().Find(info => info.itemId == _storeData.targetId);
        
        if (itemTable == null)
            return;

        txName.text = itemTable.name;

        txCost.text = _storeData.sellCost.ToString();
    }
}
