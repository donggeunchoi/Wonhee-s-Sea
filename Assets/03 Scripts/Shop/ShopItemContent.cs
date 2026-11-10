using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemContent : MonoBehaviour, IPointerClickHandler
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

    StoreData storeData;
    ItemDataTable itemTable;
    Action<ItemDataTable, Sprite> onShowItemInfo;

    public void InitData( StoreData _storeData )
    {
        storeData = _storeData;
        itemTable = null;

        if( storeData == null )
            return;

        imgIcon.sprite = SpriteManager.Ins.GetIcon(_storeData.targetId.ToString(),(EStoreType)_storeData.storeType);

        itemTable = JsonManager.Ins.GetJson<ItemDataTable>().Find(info => info.itemId == _storeData.targetId);
        txName.text = itemTable != null ? itemTable.name : string.Empty;
        txCost.text = _storeData.sellCost.ToString();
    }

    public void SetItemInfoAction( Action<ItemDataTable, Sprite> onShowItemInfo )
    {
        this.onShowItemInfo = onShowItemInfo;
    }

    public void OnPointerClick( PointerEventData eventData )
    {
        if( itemTable != null && eventData.button == PointerEventData.InputButton.Left )
            onShowItemInfo?.Invoke( itemTable, imgIcon.sprite );
    }

    public void OnClickBuyItem()
    {
        if( storeData == null || itemTable == null )
            return;

        var userManager = UserManager.Ins;
        var backendManager = BackendManager.Ins;
        if( userManager == null || userManager.userInfo == null || backendManager == null )
            return;

        int cost = storeData.sellCost;
        if( cost < 0 || userManager.userInfo.gold < cost)
        {
            Debug.Log("재화없읍");
            return;
        }

        if( userManager.AddItem( ( EItemType ) itemTable.itemType1, itemTable.itemId, 1 ) == false)
        {
            Debug.Log("아이템 없음");
            return;
        }

        if( userManager.ConsumeGood( EGoodType.Gold, cost ) == false)
        {
            Debug.Log("소모 오류");
            return;
        }

        backendManager.SaveData();
    }

}
