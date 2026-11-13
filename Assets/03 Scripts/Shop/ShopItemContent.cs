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

    [Header( "구매 버튼" )]
    [SerializeField]
    Button btBuy;

    StoreData storeData;
    ItemDataTable itemTable;
    Action<StoreData, ItemDataTable, Sprite> onShowItemInfo;

    public void InitData( StoreData _storeData )
    {
        storeData = _storeData;
        itemTable = null;

        if( storeData == null )
        {
            ReflashBuyButton();
            return;
        }

        imgIcon.sprite = SpriteManager.Ins.GetIcon(_storeData.targetId.ToString(),(EStoreType)_storeData.storeType);

        itemTable = JsonManager.Ins.GetJson<ItemDataTable>().Find(info => info.itemId == _storeData.targetId);
        txName.text = itemTable != null ? itemTable.name : string.Empty;
        txCost.text = _storeData.sellCost.ToString();
        ReflashBuyButton();
    }

    void ReflashBuyButton()
    {
        if( btBuy == null )
            return;

        btBuy.interactable = storeData != null && itemTable != null && IsOwnedSinglePurchaseItem() == false;
    }

    bool IsOwnedSinglePurchaseItem()
    {
        var userManager = UserManager.Ins;
        if( userManager == null || userManager.equipInfo == null )
            return false;

        var equipInfo = userManager.equipInfo;
        switch( ( EItemType ) itemTable.itemType1 )
        {
            case EItemType.Weapon:
                return equipInfo.TryGetWeaponLevel( itemTable.itemId, out _ );
            case EItemType.Skin:
                return equipInfo.SkinEquipData != null && equipInfo.SkinEquipData.Contains( itemTable.itemId );
        }

        return false;
    }

    public void SetItemInfoAction( Action<StoreData, ItemDataTable, Sprite> onShowItemInfo )
    {
        this.onShowItemInfo = onShowItemInfo;
    }

    public void OnPointerClick( PointerEventData eventData )
    {
        if( storeData != null && itemTable != null && eventData.button == PointerEventData.InputButton.Left )
            onShowItemInfo?.Invoke( storeData, itemTable, imgIcon.sprite );
    }

    public void OnClickBuyItem()
    {
        if( storeData == null || itemTable == null )
            return;

        if( IsOwnedSinglePurchaseItem() )
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

        EItemType itemType = ( EItemType ) itemTable.itemType1;
        int itemID = itemTable.itemId;
        if( userManager.AddItem( itemType, itemID, 1 ) == false)
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

        switch( itemType )
        {
            case EItemType.Fish:
            {
                backendManager.OnTossFish.OnNext( itemID.ToString() );
                break;
            }
            case EItemType.Weapon:
            case EItemType.Skin:
            {
                ReflashBuyButton();
                break;
            }
        }
    }

}
