using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shop_Category_BaseUI : MonoBehaviour
{
    protected Shop_PanelManager panelManager;

    public virtual void Init(Shop_PanelManager panelManager)
    {
        this.panelManager = panelManager;
    }

    protected abstract Shop_Category_UIState GetUIState();

    public virtual void SetActive(Shop_Category_UIState state)
    {
        this.gameObject.SetActive(GetUIState() == state);
    }
}
