using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTStandContent : MonoBehaviour
{
    [Header("스텐드ID")]
    [SerializeField]
    int standID;

    StandData standData = null;
    StandRecipeData standRecipeData = null;
    List<StandRecipeInputData> recipeInputDatas = new List<StandRecipeInputData>();
   

    void Awake()
    {
        FirstInit();
       if(standData == null)
        {
            var standDatas = JsonManager.Ins.GetJson<StandData>();

            standData = standDatas.Find(info => info.standId == standID);
        }

       //데이터값 잘 들어가고 있음.
        Debug.LogWarning($"스텐드 ID : {standData.standId}, 이름 : {standData.name}");
    }

    void FirstInit()
    {
        //데이터 캐싱해놓기.
        var standDatas = JsonManager.Ins.GetJson<StandData>();
        standData = standDatas.Find(info => info.standId == standID);

        var recipeDatas = JsonManager.Ins.GetJson<StandRecipeData>();
        standRecipeData = recipeDatas.Find(info => info.standId == standID);

        var inputDatas = JsonManager.Ins.GetJson<StandRecipeInputData>();
        if(recipeDatas != null)
            recipeInputDatas = inputDatas.FindAll(info => info.recipeId == standRecipeData.recipeId);

        if (standData == null)
            return;

        if (recipeDatas == null)
            return;
    }
}
