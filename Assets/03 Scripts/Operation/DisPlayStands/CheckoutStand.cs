using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutStand : MonoBehaviour
{
    public int money;
    
    public List<CustomerNPC> lineWaiters = new List<CustomerNPC>();

    public float stepDistance = 0.8f;
    public Vector3 queueDirection = Vector3.up;

    // 손님이 계산대에 도착
    // 손님의 인벤토리에 있는 물건을 확인
    // 아이템에 맞는 골드 플레이어에게 입급
    // 손님 아이템 제거

    public void Pay(string itemId, int amount)
    {
        // UserManager.Ins.userInfo.AddGold(amount);
        //경험치와 골드의 구분이 있을테니 나중에 수정할 필요가 있음.

        //경험치가 오르는것은 퀘스트를 끝내야 오르는 경우니 이 코드는 필요없는 코드.
        //다만 퀘스트가 클리어 되고있는 걸 확인할 함수가 필요할 부분이 필요 컬렉션벨류 이런걸로?
        //UserManager.Ins.userInfo.AddExp(amount);

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
            // 뒷사람들에게 한 칸씩 앞으로 오라고 신호를 보냄
            UpdateQueuePositions();
        }
    }

    void UpdateQueuePositions()
    {
        for (int i = 0; i < lineWaiters.Count; i++)
        {
            // 각 NPC에게 새로운 목적지 전달
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
