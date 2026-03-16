using UnityEngine;
using UnityEngine.UI;

public class Shop_SaleItem : MonoBehaviour
{
    [SerializeField] private Button buyButton;

    [SerializeField] private int itemid;

    private void Start()
    {
        buyButton.onClick.AddListener(OnClickBuyButton);    
    }

    public void OnClickBuyButton()
    {
        BuyItem();
    }

    public void BuyItem()
    {
       // DataInstance.Ins.Find_Id_Group(itemid);
    }
}
