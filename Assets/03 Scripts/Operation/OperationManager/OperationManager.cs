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

    // 손님 관련된 변수도 필요함 customerManager 같은거

    float currentTime;
    bool isOperating;

    public bool IsOperating => isOperating;
    public float CureentTime => currentTime;
    public event Action OnOperationStart;
    public event Action OnOperationEnd;

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
