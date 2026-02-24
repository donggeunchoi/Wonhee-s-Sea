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
            if (target is StorageStand storage)
                storage.Interact(inventory);
            else if (target is RecipeStand recipe)
                recipe.Interact(inventory);

            lastInteractTime = Time.time;
            sameTargetFrameCount = 0; // 다시 누르려면 다시 hold 필요
        }
    }

    private MonoBehaviour FindNearestTargetStand()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, range);
        if (hits == null || hits.Length == 0) return null;

        // 우선순위: StorageStand → RecipeStand (기존 로직 유지)
        for (int i = 0; i < hits.Length; i++)
        {
            var storage = hits[i].GetComponentInParent<StorageStand>();
            if (storage != null) return storage;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            var recipe = hits[i].GetComponentInParent<RecipeStand>();
            if (recipe != null) return recipe;
        }

        return null;
    }
}
