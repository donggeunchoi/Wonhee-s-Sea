using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTStandContent : MonoBehaviour
{
    [Header("스텐드ID")]
    [SerializeField]
    int standID;

    StandData standData = null;
    void Awake()
    {
       if(standData == null)
        {
            var standDatas = JsonManager.Ins.GetJson<StandData>();

            standData = standDatas.Find(info => info.standId == standID);
        }

       //데이터값 잘 들어가고 있음.
        Debug.LogWarning($"스텐드 ID : {standData.standId}, 이름 : {standData.name}");
    }

    //받을 수 있는 재료를 확인하는 함수가 필요
    //만약 해당 아이디에 맞는 재료 아이디를 가지고 있다면
    //재료를 받을 수 있도록 하는 함수가 필요

    //또... 결과물에 맞는 데이터가 필요할듯 하네

    //그리고 RecipeStand스크립트를 대체할 스크립트를 만드는 것을 목표

}
