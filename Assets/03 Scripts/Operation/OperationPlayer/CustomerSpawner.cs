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

    private float timer;
    private int aliveCount;

    void Update()
    {
        if (customerPrefab == null || targetStand == null || checkoutStand == null || exitPoint == null)
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
        Transform parent = customersContainer != null ? customersContainer : null;
        var npc = Instantiate(customerPrefab, exitPoint.position, Quaternion.identity, parent);
        npc.Init(targetStand, checkoutStand, exitPoint, colStand,colCheckOut);

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
