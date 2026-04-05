using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;

    [Header("자동 상호작용 옵션")]
    [SerializeField] private float interactCooldown = 0.2f;
    [SerializeField] private int holdFrames = 2;

    private float lastInteractTime = -999f;

    private MonoBehaviour lastTarget;
    private int sameTargetFrameCount = 0;

    void Update()
    {

        if (inventory == null) return;

        // 쿨타임 중이면 아무것도 안 함
        if (Time.time - lastInteractTime < interactCooldown)
            return;

        var target = FindNearestTargetStand();

        if (target == null)
        {
            lastTarget = null;
            sameTargetFrameCount = 0;
            return;
        }

        // 같은 대상이 연속으로 잡히는지 체크
        if (target == lastTarget)
            sameTargetFrameCount++;
        else
        {
            lastTarget = target;
            sameTargetFrameCount = 1;
        }

        // holdFrames(기본 2프레임) 이상 유지되면 1회 실행
        if (sameTargetFrameCount >= holdFrames)
        {
            InteractWithTarget(target);

            lastInteractTime = Time.time;
            sameTargetFrameCount = 0;
        }
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
        }

        // 우선순위 유지: Storage -> CT/CB -> RecipeStand
        if (bestStorage != null)
            return bestStorage;

        if (bestContentStand != null)
            return bestContentStand;

        return null;
    }
}
