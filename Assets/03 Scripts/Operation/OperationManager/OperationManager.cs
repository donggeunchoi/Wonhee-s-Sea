using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationManager : Singleton<OperationManager>
{
    [Header("운영 시간")]
    [SerializeField]
    float operationDuration;

    [Header("연결")]
    [SerializeField]
    CheckoutStand checkoutStand;

    [Header("CTStand")]
    [SerializeField]
    List<CTStandContent> cTStandContent = new List<CTStandContent>();

    [Header("CBStand")]
    [SerializeField]
    List<CBStandContent> cBStandContent = new List<CBStandContent>();

    [Header("DSSaShimiStand")]
    [SerializeField]
    List<StorageStand> dSSaShimiStandContent = new List<StorageStand>();

    [Header("DSSushiStandContent")]
    [SerializeField]
    List<StorageStand> dSSushiStandContent = new List<StorageStand>();

    // 손님 관련된 변수도 필요함 customerManager 같은거


    float currentTime;
    bool isOperating;

    public bool IsOperating => isOperating;
    public float CureentTime => currentTime;
    public event Action OnOperationStart;
    public event Action OnOperationEnd;

    void Awake()
    {
        var standDatas = JsonManager.Ins.GetJson<StandData>();

        foreach(var standGroup in standDatas)
        {
            if(standGroup.stage == UserManager.Ins.userInfo.currentStage)
            {
                switch (standGroup.standType1)
                {
                    case "1":
                    {
                         for (int i = 0; i < cTStandContent.Count; i++)
                         {
                              cTStandContent[i].standID = standGroup.standId;
                         }
                    }
                    break;
                    case "2":
                    {
                       for (int i = 0; i < cBStandContent.Count; i++)
                       {
                           cBStandContent[i].standID = standGroup.standId;
                       }
                    }
                    break;
                    case "3":
                    {
                        for (int i = 0; i < dSSaShimiStandContent.Count; i++)
                        {
                            dSSaShimiStandContent[i].standID = standGroup.standId;
                        }
                     }
                    break;
                    case "4":
                    {
                        for (int i = 0; i < dSSushiStandContent.Count; i++)
                        {
                            dSSushiStandContent[i].standID = standGroup.standId;
                        }
                    }
                    break;

                }
                
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartOperation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOperating)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= operationDuration)
        {
            EndOperation();
        }
    }

    public void StartOperation()
    {
        if (isOperating)
            return;

        currentTime = 0f;
        isOperating = true;

        OnOperationStart?.Invoke();

        Debug.Log("[OperationManager] 영업 시작");
    }

    public void EndOperation()
    {
        if (!isOperating)
            return;

        isOperating = false;

        OnOperationEnd?.Invoke();
        //BackendManager.Ins.SaveData();

    }
}
