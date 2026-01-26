using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private PlayerStackVisualizer stack;

    void Update()
    {
        
        var hits = Physics2D.OverlapCircleAll(transform.position, range);
        if (hits == null) return;
        
        foreach(var hit in hits)
        {
            var storage = hit.GetComponentInParent<StorageStand>();
            if (storage != null)
            {
                storage.Interact(inventory);
                return;
            }
        }

        foreach (var hit in hits)
        {
            // RecipeStand 그 다음
            var recipe = hit.GetComponentInParent<RecipeStand>();
            if (recipe != null)
            {
                recipe.Interact(inventory, stack);
                return;
            }
        }

    }
}
