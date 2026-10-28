using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("골드")]
    [SerializeField]
    TextMeshProUGUI txGold;
    [Header("다이아")]
    [SerializeField]
    TextMeshProUGUI txDia;
    [Header("강화석")]
    [SerializeField]
    TextMeshProUGUI txEnchant;
    [Header("레벨")]
    [SerializeField]
    TextMeshProUGUI txLevel;
    [Header("평판")]
    [SerializeField]
    TextMeshProUGUI txRating;

    [Header("상점 팝업")]
    [SerializeField]
    GameObject storePopup;

    private void Awake()
    {
        txLevel.text = string.Format("Lv.{0}", UserManager.Ins.userInfo.level);
        txGold.text = string.Format("골드:{0}", UserManager.Ins.userInfo.gold);
        txRating.text = string.Format("평판:{0}", UserManager.Ins.userInfo.exp);

        //UserInfo 다이아,강화석 추가 필요
        txDia.text = string.Format("dia:{0}", UserManager.Ins.userInfo.dia);
        txEnchant.text = string.Format("enchant:{0}", UserManager.Ins.userInfo.enchant);
    }

    public void OnClickStore()
    {
        Debug.LogWarning("상점 버튼 눌림");
        storePopup.SetActive(true);
    }

    public void OnClickTown()
    {
        Debug.LogWarning("타운 버튼 눌림");
    }

    public void OnClickDungeonScene()
    {
        UserManager.Ins.SetDungeonSpawnFishIdsFromPlacedStands();
        SceneManager.LoadScene("Gwang_DungeonScene");
    }

    public void OnClickSetting()
    {
        Debug.LogWarning("설정 버튼 눌림");
    }
}
