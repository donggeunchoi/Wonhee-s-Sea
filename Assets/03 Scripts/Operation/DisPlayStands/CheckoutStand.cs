
using System.Collections.Generic;
using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public int money;

    [Header("계산대 대기열")]
    public List<CustomerNPC> lineWaiters
        = new List<CustomerNPC>();

    [Header("계산대로 이동 중인 손님")]
    [SerializeField]
    private List<CustomerNPC> approachingCustomers
        = new List<CustomerNPC>();

    [Header("대기열 설정")]
    public float stepDistance = 0.8f;
    public Vector3 queueDirection = Vector3.up;


    #region 결제 처리

    public void Pay(string itemId, int amount)
    {
        if (amount <= 0)
            return;

        if (!TryGetItemPrice(itemId, out int price))
            return;

        int gold = price * amount;

        UserManager.Ins.userInfo.AddGold(gold);
    }

    private bool TryGetItemPrice(string itemId, out int price)
    {
        price = 0;

        if (!int.TryParse(itemId, out int ingredientId))
        {
            Debug.LogWarning(
                $"[CheckoutStand] Invalid itemId: {itemId}",
                this
            );

            return false;
        }

        IngredientData data =
            JsonManager.Ins.GetDataByKey<IngredientData>(
                ingredientId,
                ingredient => ingredient.ingredientId
            );

        if (data == null)
        {
            Debug.LogWarning(
                $"[CheckoutStand] Item price not found: {itemId}",
                this
            );

            return false;
        }

        if (data.cost <= 0)
        {
            Debug.LogWarning(
                $"[CheckoutStand] Invalid item cost: {data.cost}",
                this
            );

            return false;
        }

        price = data.cost;

        return true;
    }

    #endregion


    #region 대기열 좌표 계산

    // 순번에 해당하는 대기 위치
    private Vector3 GetQueuePosition(int index)
    {
        return transform.position
            + queueDirection.normalized
            * stepDistance
            * (index + 1);
    }

    // 현재 대기열의 마지막 빈자리
    public Vector3 GetQueueTailPosition()
    {
        return GetQueuePosition(lineWaiters.Count);
    }

    #endregion


    #region 계산대로 이동 중인 손님 관리

    public Vector3 RegisterApproachingCustomer(CustomerNPC npc)
    {
        if (!approachingCustomers.Contains(npc) &&
            !lineWaiters.Contains(npc))
        {
            approachingCustomers.Add(npc);
        }

        return GetQueueTailPosition();
    }

    private void UpdateApproachingCustomers()
    {
        Vector3 newTarget = GetQueueTailPosition();

        foreach (CustomerNPC npc in approachingCustomers)
        {
            if (npc == null)
                continue;

            npc.UpdateCheckoutApproachTarget(newTarget);
        }
    }

    #endregion


    #region 실제 대기열 관리

    // 실제 마지막 빈자리에 도착했을 때 호출
    public Vector3 JoinQueue(CustomerNPC npc)
    {
        // 이동 중인 손님 리스트에서 제거
        approachingCustomers.Remove(npc);

        // 실제 대기열 등록
        if (!lineWaiters.Contains(npc))
        {
            lineWaiters.Add(npc);

            // 마지막 빈자리가 변경되었으므로
            // 이동 중인 손님들의 목적지 갱신
            UpdateApproachingCustomers();
        }

        int myIndex = lineWaiters.IndexOf(npc);

        return GetQueuePosition(myIndex);
    }

    // 계산대에서 떠나는 손님 제거
    public void LeaveQueue(CustomerNPC npc)
    {
        // 이동 중이었다면 리스트에서 제거
        approachingCustomers.Remove(npc);

        // 실제 대기열에 등록되어 있었다면 제거
        if (lineWaiters.Remove(npc))
        {
            // 기존 대기열 손님들의 위치 갱신
            UpdateQueuePositions();

            // 계산대로 이동 중인 손님들의 위치 갱신
            UpdateApproachingCustomers();
        }
    }

    // 줄을 서고 있는 손님들의 위치 갱신
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < lineWaiters.Count; i++)
        {
            CustomerNPC npc = lineWaiters[i];

            if (npc == null)
                continue;

            Vector3 newPos = GetQueuePosition(i);

            npc.UpdateQueueTarget(newPos);
        }
    }

    // 첫 번째 순번인지 확인
    public bool IsFirstInLine(CustomerNPC npc)
    {
        return lineWaiters.Count > 0 &&
               lineWaiters[0] == npc;
    }

    #endregion
}
