using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("스폰할 손님 프리팹")]
    [SerializeField] 
    private CustomerNPC customerPrefab;

    [Header("참조")]
    [SerializeField] 
    private StorageStand targetStand;
    [SerializeField] 
    private CheckoutStand checkoutStand;

    [Header("스폰/퇴장 지점 (ExitPoint)")]
    [SerializeField] 
    private Transform exitPoint;

    [Header("스폰 주기(초)")]
    [SerializeField] 
    private float spawnInterval;

    [Header("동시에 존재 가능한 손님 수")]
    [SerializeField] 
    private int maxAlive;

    [Header("손님 담을 컨테이너")]
    [SerializeField]
    private Transform customersContainer;

    [Header("충돌 배당")]
    [SerializeField]
    Collider2D colStand;
    [SerializeField]
    Collider2D colCheckOut;

    [Header("길찾기 로직")]
    [SerializeField] PathFinding pathFinding;

    private float timer;
    private int aliveCount;

    void Update()
    {
        if (customerPrefab == null || checkoutStand == null || exitPoint == null)
            return;

        if (aliveCount >= maxAlive)
            return;

        timer += Time.deltaTime;
        if (timer < spawnInterval)
            return;

        timer = 0f;
        SpawnCustomer();

        if(customersContainer.childCount == 0)
        {
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        // 해금된 stnad id를 통해 판매 물품 id 가져올 예정
        int random = Random.Range(30002, 30005); // 임시
        string requestedItemId = random.ToString();

        targetStand = StandManager.Ins.GetStandByItemId(requestedItemId);
        StorageStand matchedStand = targetStand;

        if (targetStand == null)
        {
            Debug.Log("target stand가 존재하지 않습니다");
        }

        Transform parent = customersContainer != null ? customersContainer : null;
        var npc = Instantiate(customerPrefab, exitPoint.position, Quaternion.identity, parent);
        npc.Init(targetStand, checkoutStand, exitPoint, colStand,colCheckOut, pathFinding);

        Inventory npcInventory = npc.GetComponent<Inventory>();
        if (npcInventory != null)
        {
            npcInventory.InitializeInventoryWithItem(matchedStand.SellItemId, 1);
        }

        // 손님이 사라질 때 aliveCount 줄이기
        //npc.OnDespawn += HandleDespawn;

        aliveCount++;
    }

    private void HandleDespawn(CustomerNPC npc)
    {
       // npc.OnDespawn -= HandleDespawn;
        aliveCount = Mathf.Max(0, aliveCount - 1);
    }
}
