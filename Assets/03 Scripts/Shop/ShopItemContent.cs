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

    ItemDataTable itemTable;
    Action<ItemDataTable, Sprite> onShowItemInfo;

    public void InitData(StoreData _storeData)
    {
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
        //여기에는 구매하기 기능을 넣을것이고 구매를 할때 액션만 넘길것인지
        // 구매를 할 수 있는 코드 자체를 넣을것인지
        // 내가볼때는 구매한다는 액션만 넣으면 될꺼같고 이 처리는 팝업에서 처리하면 될꺼같은데.
        // 저장은 BackendManager.Ins.EquipInfo를 통해서 아이템 저장 을 넣으면 되지 않을까?
        //
    }
}
