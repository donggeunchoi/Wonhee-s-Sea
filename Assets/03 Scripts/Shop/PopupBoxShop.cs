using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[Serializable]
public class PopupBoxStoreTap
{
    public EStoreType storeType;
    public GameObject btOn;
    public Button btOff;
}

public class PopupBoxShop : MonoBehaviour
{
    [Header("상단탭")]
    [SerializeField]
    List<PopupBoxStoreTap> storeTaps = new List<PopupBoxStoreTap>();

    [Header("장비, 재료 UI")]
    [SerializeField]
    GameObject storeScrollView;

    [Header("직원, 스킨 UI")]
    [SerializeField]
    GameObject EmloyeeScrollView;

    [Header("장비, 재료 아이템")]
    [SerializeField]
    ShopItemContent itemContent;
    [SerializeField]
    Transform itemTransform;

    EStoreType currentType = EStoreType.Equipment;

    List<StoreData> storeDatas = new List<StoreData>();
    List<ShopItemContent> shopItemContents = new List<ShopItemContent>();

    private void Awake()
    {
        storeDatas = JsonManager.Ins.GetJson<StoreData>();
        foreach (var tap in storeTaps)
        {
            tap.btOff.onClick.AsObservable().Subscribe(_ =>
            {
                InitData(tap.storeType);
            }).AddTo(this);
        }
    }

    private void OnEnable()
    {
        InitData(currentType);
    }


    void InitData(EStoreType type)
    {
        var storeJsons = storeDatas.FindAll(info => info.storeType == (int)type);

        currentType = type;
        foreach (var tap in storeTaps)
        {
            if (tap.storeType == type)
            {
                tap.btOn.SetActive(true);
            }
            else
            {
                tap.btOn.SetActive(false);
            }
        }

        switch (currentType)
        {
            case EStoreType.Equipment:
            case EStoreType.Ingredient:
            {
                storeScrollView.SetActive(true);
                EmloyeeScrollView.SetActive(false);

                for (int i = 0; i < storeJsons.Count; i++)
                {
                    if (i >= shopItemContents.Count)
                    {
                        var shopItem = Instantiate(itemContent, itemTransform, false);
                        shopItemContents.Add(shopItem);
                    }

                    var content = shopItemContents[i];
                    content.InitData(storeJsons[i]);
                    content.gameObject.SetActive(true);
                }

                for (int i = storeJsons.Count; i < shopItemContents.Count; i++)
                {
                    shopItemContents[i].gameObject.SetActive(false);
                }
            }
            break;

            case EStoreType.Employee:
            case EStoreType.Skin:
            {
                storeScrollView.SetActive(false);
                EmloyeeScrollView.SetActive(true);
            }
            break;
        }
    }

}
