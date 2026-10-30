using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;

    [Header("자동 상호작용 옵션")]
    [SerializeField] private float interactCooldown ;
    [SerializeField] private int holdFrames;

    [Header("최초 접근 대기 시간")]
    [SerializeField]
    private float interactDelay = 1f;

    private float targetStayTime;
    private float lastInteractTime;

    private MonoBehaviour lastTarget;
    private int sameTargetFrameCount;
    private Container activeContainer;

    void Update()
    {
        if (inventory == null)
            return;

        var target = FindNearestTargetStand();

        if (target == null)
        {
            if (activeContainer != null)
            {
                activeContainer.SetVisualActive(false);
                activeContainer = null;
            }

            lastTarget = null;
            sameTargetFrameCount = 0;
            targetStayTime = 0f;
            return;
        }

        if (target != lastTarget)
        {
            lastTarget = target;
            sameTargetFrameCount = 1;
            targetStayTime = 0f;

            // 접근 대기가 끝나면 첫 상호작용을 바로 실행할 수 있도록 한다.
            lastInteractTime = Time.time - interactCooldown;
            return;
        }

        sameTargetFrameCount++;
        targetStayTime += Time.deltaTime;

        if (sameTargetFrameCount < holdFrames)
            return;

        if (targetStayTime < interactDelay)
            return;

        if (Time.time - lastInteractTime < interactCooldown)
            return;

        InteractWithTarget(target);
        lastInteractTime = Time.time;
    }

    private void InteractWithTarget(MonoBehaviour target)
    {
        if (target == null)
            return;

        if (target is StorageStand storage)
        {
            storage.Interact(inventory);
            return;
        }

        if (target is CTStandContent ctStand)
        {
            ctStand.Interact(inventory);
            return;
        }

        if (target is CBStandContent cbStand)
        {
            cbStand.Interact(inventory);
            return;
        }

        if (target is Container container)
        {
            if (activeContainer != null && activeContainer != container)
                activeContainer.SetVisualActive(false);

            activeContainer = container;
            container.Interact();
            return;
        }
    }

    private MonoBehaviour FindNearestTargetStand()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        
        if (hits == null || hits.Length == 0)
            return null;

        MonoBehaviour bestStorage = null;
        float bestStorageDist = float.MaxValue;

        MonoBehaviour bestContentStand = null;
        float bestContentStandDist = float.MaxValue;

        Vector2 myPos = transform.position;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            Transform targetTransform = hits[i].transform;
            float dist = Vector2.Distance(myPos, targetTransform.position);

            StorageStand storage = hits[i].GetComponentInParent<StorageStand>();
            if (storage != null && dist < bestStorageDist)
            {
                bestStorage = storage;
                bestStorageDist = dist;
                continue;
            }

            CTStandContent ctStand = hits[i].GetComponentInParent<CTStandContent>();
            if (ctStand != null && dist < bestContentStandDist)
            {
                bestContentStand = ctStand;
                bestContentStandDist = dist;
                continue;
            }

            CBStandContent cbStand = hits[i].GetComponentInParent<CBStandContent>();
            if (cbStand != null && dist < bestContentStandDist)
            {
                bestContentStand = cbStand;
                bestContentStandDist = dist;
                continue;
            }

            Container container = hits[i].GetComponentInParent<Container>();
            if (container != null && dist < bestContentStandDist)
            {
                bestContentStand = container;
                bestContentStandDist = dist;
                continue;
            }
        }

        // 우선순위 유지: Storage -> CT/CB -> RecipeStand
        if (bestStorage != null)
            return bestStorage;

        if (bestContentStand != null)
            return bestContentStand;

        return null;
    }
}
