using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Category_EquipmentUI : Shop_Category_BaseUI
{
    [SerializeField] private GameObject equipment_Shop;

    public override void Init(Shop_PanelManager panelManager)
    {
        base.Init(panelManager);
    }

    protected override Shop_Category_UIState GetUIState()
    {
        return Shop_Category_UIState.Equipment;
    }

    public override void SetActive(Shop_Category_UIState state)
    {
        equipment_Shop.SetActive(GetUIState() == state);
    }
}
