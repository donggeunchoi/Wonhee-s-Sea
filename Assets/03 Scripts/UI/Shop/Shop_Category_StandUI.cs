using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Category_StandUI : Shop_Category_BaseUI
{
    [SerializeField] private GameObject ingredient_Shop;

    public override void Init(Shop_PanelManager panelManager)
    {
        base.Init(panelManager);
    }

    protected override Shop_Category_UIState GetUIState()
    {
        return Shop_Category_UIState.Stand;
    }

    public override void SetActive(Shop_Category_UIState state)
    {
        ingredient_Shop.SetActive(GetUIState() == state);
    }
}
