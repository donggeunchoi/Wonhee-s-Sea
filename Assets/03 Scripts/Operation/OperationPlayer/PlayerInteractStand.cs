using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var hit = Physics2D.OverlapCircle(transform.position, range);
        if (hit == null) return;

        
        if (!hit.TryGetComponent<SashimiStand>(out var stand))
            return;
        
        //stand.Interact(inventory);
    }
}
