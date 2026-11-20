using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("골드")]
    [SerializeField]
    TextMeshProUGUI txGold;
    [SerializeField]
    Image imgGold;

    [Header("다이아")]
    [SerializeField]
    TextMeshProUGUI txDia;
    [SerializeField]
    Image imgDia;

    [Header("강화석")]
    [SerializeField]
    TextMeshProUGUI txEnchant;
    [SerializeField]
    Image imgEnchant;

    [Header("레벨")]
    [SerializeField]
    TextMeshProUGUI txLevel;
    [Header("평판")]
    [SerializeField]
    TextMeshProUGUI txRating;

    [Header("상점 팝업")]
    [SerializeField]
    GameObject storePopup;

    [Header( "설정 팝업" )]
    [SerializeField]
    GameObject popupBoxSetting;

    private void Awake()
    {
        UserManager.Ins.OnResources.AsObservable().Subscribe(_ =>
        {
            InitData();
        }).AddTo(this);

        InitData();
    }

    void InitData()
    {
        txLevel.text = string.Format("Lv.{0}", UserManager.Ins.userInfo.level);
        txGold.text = string.Format("{0}", UserManager.Ins.userInfo.gold);
        txRating.text = string.Format("{0}", UserManager.Ins.userInfo.exp);

        //UserInfo 다이아,강화석 추가 필요
        txDia.text = string.Format("{0}", UserManager.Ins.userInfo.dia);
        txEnchant.text = string.Format("{0}", UserManager.Ins.userInfo.enchant);

        imgGold.sprite = SpriteManager.Ins.OnGetSprite(EItemType.Gold);
        imgDia.sprite = SpriteManager.Ins.OnGetSprite(EItemType.Dia);
        imgEnchant.sprite = SpriteManager.Ins.OnGetSprite(EItemType.Enchant);
    }

    public void OnClickStore()
    {
        Debug.LogWarning("상점 버튼 눌림");
        storePopup.SetActive(true);
    }

    public void OnClickTown()
    {
        Debug.LogWarning("타운 버튼 눌림");
        storePopup.SetActive(false);
    }

    public void OnClickDungeonScene()
    {
        UserManager.Ins.SetDungeonSpawnFishIdsFromPlacedStands();
        SceneManager.LoadScene("Gwang_DungeonScene");
    }

    public void OnClickSetting()
    {
        if( popupBoxSetting != null )
            popupBoxSetting.SetActive( true );
    }

    public void OnClickCloseSetting()
    {
        if( popupBoxSetting != null )
            popupBoxSetting.SetActive( false );
    }
}
