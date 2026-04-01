using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public int money;
    
    public List<CustomerNPC> lineWaiters = new List<CustomerNPC>();

    public float stepDistance = 0.8f;
    public Vector3 queueDirection = Vector3.up;

    public void Pay(string itemId, int amount)
    {
        // 일단은 단순히 돈만 증가(가격은 DB에서 읽어도 됨)
        money += amount * 1;
        Debug.Log($"결제 완료: {itemId} x{amount}, money={money}");
    }

    public Vector3 JoinQueue(CustomerNPC npc)
    {
        if (!lineWaiters.Contains(npc))
        {
            lineWaiters.Add(npc);
        }

        int myIndex = lineWaiters.IndexOf(npc);

        Debug.Log(transform.position + (queueDirection * stepDistance * (myIndex + 1)));

        return transform.position + (queueDirection * stepDistance * (myIndex + 1));
    }

    // 계산 완료 후 줄에서 빠지기
    public void LeaveQueue(CustomerNPC npc)
    {
        if (lineWaiters.Contains(npc))
        {
            lineWaiters.Remove(npc);
            // 뒷사람들에게 한 칸씩 앞으로 오라고 신호를 보냅니다.
            UpdateQueuePositions();
        }
    }

    void UpdateQueuePositions()
    {
        for (int i = 0; i < lineWaiters.Count; i++)
        {
            // 각 NPC에게 새로운 목적지를 전달합니다.
            Vector3 newPos = transform.position + (queueDirection * stepDistance * (i + 1));
            lineWaiters[i].UpdateQueueTarget(newPos);
        }
    }

    public bool IsFirstInLine(CustomerNPC npc)
    {
        if (lineWaiters.Count > 0 && lineWaiters[0] == npc)
        {
            return true;
        }
        return false;
    }

}
