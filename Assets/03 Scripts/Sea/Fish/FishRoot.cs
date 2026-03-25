using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRoot : MonoBehaviour
{
    // 임시
    public int fishId;

    public Rigidbody2D Rb { get; private set; }
    public FishController Controller { get; private set; }
    public FishStats Stats { get; private set; }
    public FishHpUI HpUI { get; private set; }

    public Transform PlayerTr { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<FishController>();
        Stats = GetComponent<FishStats>();
        HpUI = GetComponentInChildren<FishHpUI>();

        Controller?.Init(this);
        Stats?.Init(this);
        HpUI?.Init(Stats);
    }

    public void Init(Transform playerTr)
    {
        PlayerTr = playerTr;
        Controller?.BindTarget(PlayerTr);
    }
}
