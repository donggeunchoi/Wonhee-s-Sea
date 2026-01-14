using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandView : MonoBehaviour
{
    [SerializeField] private ItemDatabaseSO db;
    
    [Header("결과물 아이콘")]
    [SerializeField] private Image imgOutputIcon;
    [SerializeField] private GameObject objOutputGroup;

    [Header("제작중 표시")]
    [SerializeField] private GameObject objProcessingGroup;
    [SerializeField] private Image imgProgressFill;
    [SerializeField] private TextMeshProUGUI txtRemainTime;

    private float processDuration;
    private float remainTime;
    private bool isProcessing;
    
    
    
    public void ShowOutput(string itemId)
    {
        if (objOutputGroup == null) return;

        if (string.IsNullOrEmpty(itemId))
        {
            objOutputGroup.SetActive(false);
            
            return;
        }

        objOutputGroup.SetActive(true);
        
    }
    
    public void ShowProcessing(bool on, float duration = 0f)
    {
        isProcessing = on;
        objProcessingGroup?.SetActive(isProcessing);

        if (!on)
        {
            if (imgProgressFill != null)
                imgProgressFill.fillAmount = 0f;

            if (txtRemainTime != null)
                txtRemainTime.text = "";
            return;
        }

        processDuration = duration;
        remainTime = duration;
    }
}
