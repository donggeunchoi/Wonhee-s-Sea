using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
   
    [SerializeField] float range = 1.5f;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var stand = StandManager.Ins.GetNearest(transform.position, range);
        if (stand == null) return;

        var ctx = new StandManager.StandContext
        {
            
        };

        stand.Interact(ctx);
    }
}
