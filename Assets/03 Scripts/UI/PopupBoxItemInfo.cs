using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemInfoEffect
{
    public Sprite icon;
    public string name;
    public string value;
}

[Serializable]
public class ItemInfoPopupData
{
    public Sprite icon;
    public string category;
    public string name;
    public string description;
    public int ownedCount;
    public bool hasOwnedCount = true;
    public bool isEquipped;
    public bool canEquip;
    public bool canUnequip;
    public bool hasUpgrade;
    public bool canUpgrade;
    public int level;
    public Sprite upgradeCostIcon;
    public string upgradeCost;
    public Sprite extraUpgradeCostIcon;
    public string extraUpgradeCost;
    public List<ItemInfoEffect> effects = new List<ItemInfoEffect>();
}

public class PopupBoxItemInfo : MonoBehaviour
{
    [Serializable]
    class EffectRow
    {
        public GameObject objRow;
        public Image imgIcon;
        public TextMeshProUGUI txName;
        public TextMeshProUGUI txValue;
    }

    [Header( "아이템 정보" )]
    [SerializeField]
    Image imgIcon;
    [SerializeField]
    GameObject objEquipped;
    [SerializeField]
    TextMeshProUGUI txCategory;
    [SerializeField]
    TextMeshProUGUI txOwnedCount;
    [SerializeField]
    TextMeshProUGUI txName;
    [SerializeField]
    TextMeshProUGUI txDescription;

    [Header( "효과" )]
    [SerializeField]
    GameObject objEffects;
    [SerializeField]
    List<EffectRow> effectRows = new List<EffectRow>();

    [Header( "강화" )]
    [SerializeField]
    RectTransform rectUpgrade;
    [SerializeField]
    TextMeshProUGUI txUpgradeLevel;
    [SerializeField]
    Image imgUpgradeCost;
    [SerializeField]
    TextMeshProUGUI txUpgradeCost;
    [SerializeField]
    Image imgExtraUpgradeCost;
    [SerializeField]
    TextMeshProUGUI txExtraUpgradeCost;
    [SerializeField]
    Button btUpgrade;

    [Header( "하단 버튼" )]
    [SerializeField]
    Button btEquip;
    [SerializeField]
    Button btUnequip;
    [SerializeField]
    RectTransform rectFooter;
    [SerializeField]
    AspectRatioFitter panelFitter;
    [SerializeField]
    RectTransform rectBody;
    [SerializeField]
    float bodyHeight = 910;
    [SerializeField]
    float upgradeHeight = 416;
    [SerializeField]
    float footerHeight = 118;
    [SerializeField]
    float panelWidth = 704;

    Action onEquip;
    Action onUnequip;
    Action onUpgrade;

    public void InitData( ItemInfoPopupData data, Action onEquip = null, Action onUnequip = null, Action onUpgrade = null )
    {
        const float COMPACT_BODY_HEIGHT = 560;

        if( data == null )
            return;

        this.onEquip = onEquip;
        this.onUnequip = onUnequip;
        this.onUpgrade = onUpgrade;

        imgIcon.sprite = data.icon;
        imgIcon.enabled = data.icon != null;
        objEquipped.SetActive( data.isEquipped );
        txCategory.text = data.category;
        txCategory.transform.parent.gameObject.SetActive( string.IsNullOrEmpty( data.category ) == false );
        txOwnedCount.text = string.Format( "보유 {0:N0}개", data.ownedCount );
        txOwnedCount.transform.parent.gameObject.SetActive( data.hasOwnedCount );
        txName.text = data.name;
        txDescription.text = data.description;
        txDescription.gameObject.SetActive( string.IsNullOrEmpty( data.description ) == false );

        int visibleCount = 0;
        if( data.effects != null )
        {
            foreach( var effect in data.effects )
            {
                if( effect == null )
                    continue;

                if( visibleCount >= effectRows.Count )
                    break;

                var row = effectRows[ visibleCount++ ];
                row.imgIcon.sprite = effect.icon;
                row.imgIcon.enabled = effect.icon != null;
                row.imgIcon.color = Color.white;
                row.txName.text = effect.name;
                row.txValue.text = effect.value;
                row.objRow.SetActive( true );
            }
        }

        for( int i = visibleCount; i < effectRows.Count; i++ )
        {
            effectRows[ i ].objRow.SetActive( false );
        }

        objEffects.SetActive( visibleCount > 0 );
        rectUpgrade.gameObject.SetActive( data.hasUpgrade );
        if( data.hasUpgrade )
        {
            txUpgradeLevel.text = string.Format( "Lv. {0} → <color=#087BFF>Lv. {1}</color>", data.level, data.level + 1 );
            imgUpgradeCost.sprite = data.upgradeCostIcon;
            imgUpgradeCost.enabled = data.upgradeCostIcon != null;
            imgUpgradeCost.color = Color.white;
            txUpgradeCost.text = data.upgradeCost;
            imgExtraUpgradeCost.sprite = data.extraUpgradeCostIcon;
            imgExtraUpgradeCost.enabled = data.extraUpgradeCostIcon != null;
            imgExtraUpgradeCost.color = Color.white;
            txExtraUpgradeCost.text = data.extraUpgradeCost;
        }

        btUpgrade.interactable = data.hasUpgrade && data.canUpgrade && onUpgrade != null;
        btEquip.gameObject.SetActive( data.canEquip && data.isEquipped == false );
        btEquip.interactable = onEquip != null;
        btUnequip.gameObject.SetActive( data.canUnequip && data.isEquipped );
        btUnequip.interactable = onUnequip != null;
        bool hasFooter = btEquip.gameObject.activeSelf || btUnequip.gameObject.activeSelf;
        rectFooter.gameObject.SetActive( hasFooter );

        float currentBodyHeight = visibleCount > 0 ? bodyHeight : COMPACT_BODY_HEIGHT;
        float panelHeight = currentBodyHeight + ( data.hasUpgrade ? upgradeHeight : 0 ) + ( hasFooter ? footerHeight : 0 );
        panelFitter.aspectRatio = panelWidth / panelHeight;
        float bodyBottom = 1 - currentBodyHeight / panelHeight;
        rectBody.anchorMin = new Vector2( 0, bodyBottom );
        rectUpgrade.anchorMin = new Vector2( 0, hasFooter ? footerHeight / panelHeight : 0 );
        rectUpgrade.anchorMax = new Vector2( 1, bodyBottom );
        rectFooter.anchorMax = new Vector2( 1, hasFooter ? footerHeight / panelHeight : 0 );

        if( hasFooter == false )
            return;

        var rectAction = ( RectTransform )( btEquip.gameObject.activeSelf ? btEquip.transform : btUnequip.transform );
        rectAction.anchorMin = new Vector2( 0.075f, rectAction.anchorMin.y );
        rectAction.anchorMax = new Vector2( 0.925f, rectAction.anchorMax.y );
    }

    public void OnClickClose()
    {
        gameObject.SetActive( false );
    }

    public void OnClickEquip()
    {
        if( btEquip.isActiveAndEnabled && btEquip.IsInteractable() )
            onEquip?.Invoke();
    }

    public void OnClickUnequip()
    {
        if( btUnequip.isActiveAndEnabled && btUnequip.IsInteractable() )
            onUnequip?.Invoke();
    }

    public void OnClickUpgrade()
    {
        if( btUpgrade.isActiveAndEnabled && btUpgrade.IsInteractable() )
            onUpgrade?.Invoke();
    }
}
