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

    EStoreType currentType = EStoreType.Equipment;

    private void Awake()
    {
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
