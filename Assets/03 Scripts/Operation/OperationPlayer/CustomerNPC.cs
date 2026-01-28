using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CustomerNPC : MonoBehaviour
{
    [Header("이동")]
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float arriveDist = 0.1f;

    [Header("참조")]
    [SerializeField]
    private Inventory npcInventory;
    [SerializeField]
    private StorageStand targetStand;
    [SerializeField]
    private CheckoutStand checkout;
    [SerializeField]
    private Transform exitPoint;

    private ENPCState state;

    // Start is called before the first frame update
    void Start()
    {
        state = ENPCState.MoveToStand;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case ENPCState.MoveToStand:
                {
                    MoveTo(targetStand.transform.position, ENPCState.Pick);
                }
                break;

            case ENPCState.Pick:
                {
                    TryPickOne();
                }
                break;

            case ENPCState.MoveToCheckout:
                {
                    MoveTo(checkout.transform.position, ENPCState.Pay);
                }
                break;

            case ENPCState.Pay:
                {
                    DoPay();
                }
                break;

            case ENPCState.Exit:
                {
                    MoveTo(exitPoint.position, nullState: true);
                }
                break;
        }
    }

    void MoveTo(Vector3 target, ENPCState? next = null, bool nullState = false)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position,target) <= arriveDist)
        {
            if(nullState)
            {
                Destroy(gameObject);
                return;
            }
            if(next.HasValue)
            {
                ENPCState state = next.Value;
            }
        }
    }

    void TryPickOne()
    {
        // 재고 없으면 Exit
        if (targetStand == null || !targetStand.HasStock(1))
        {
            state = ENPCState.Exit;
            return;
        }

        // 꺼내기 실패하면 Exit (여기 ! 중요)
        if (!targetStand.TryTake(1))
        {
            state = ENPCState.Exit;
            return;
        }

        // 성공하면 손님 인벤에 추가
        string itemId = targetStand.SellItemId;
        if (npcInventory != null)
            npcInventory.TryAdd(itemId, 1);

        // 다음 상태로 이동
        state = ENPCState.MoveToCheckout;
    }

    void DoPay()
    {
        string itemId = targetStand.SellItemId;

        // 손님 인벤에서 제거(머리위 자동 제거)
        if (npcInventory != null)
            npcInventory.TryRemove(itemId, 1);

        checkout.Pay(itemId, 1);
        state = ENPCState.Exit;
    }

    // 외부에서 세팅용
    public void Init(StorageStand stand, CheckoutStand checkoutStand, Transform exit)
    {
        targetStand = stand;
        checkout = checkoutStand;
        exitPoint = exit;
    }

}
