using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }

    public PlayerInputHandler Input { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerAttackController Attack { get; private set; }
    public PlayerInventory Inventory { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();

        Input = GetComponent<PlayerInputHandler>();
        Controller = GetComponent<PlayerController>();
        Attack = GetComponent<PlayerAttackController>();
        Inventory = GetComponent<PlayerInventory>();

        Input?.Init(this);
        Controller?.Init(this);
        Attack?.Init(this);
    }
}
