using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRoot : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }
    public FishController Controller { get; private set; }
    public FishStats Stats { get; private set; }

    public Transform PlayerTr { get; private set; }
    public PlayerStats PlayerStats { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<FishController>();
        Stats = GetComponent<FishStats>();

        Controller?.Init(this);
        Stats?.Init(this);
    }

    public void Init(Transform playerTr)
    {
        PlayerTr = playerTr;
        PlayerStats = playerTr != null ? playerTr.GetComponent<PlayerStats>() : null;

        Controller?.BindTarget(PlayerTr, PlayerStats);
    }
}
