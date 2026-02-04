using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Category_EmplyoeeUI : Shop_Category_BaseUI
{
    [SerializeField] private GameObject Employee_Shop;

    public override void Init(Shop_PanelManager panelManager)
    {
        base.Init(panelManager);
    }

    protected override Shop_Category_UIState GetUIState()
    {
        return Shop_Category_UIState.Employee;
    }

    public override void SetActive(Shop_Category_UIState state)
    {
        Employee_Shop.SetActive(GetUIState() == state);
    }
}
