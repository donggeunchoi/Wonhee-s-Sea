using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRoot : MonoBehaviour
{
    [SerializeField] private int _fishId;

    public int FishId => _fishId;
    public FishData Data { get; private set; }

    public Rigidbody2D Rb { get; private set; }
    public FishController Controller { get; private set; }
    public FishStats Stats { get; private set; }
    public FishHpUI HpUI { get; private set; }
    public FishSensor Sensor { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<FishController>();
        Stats = GetComponent<FishStats>();
        HpUI = GetComponentInChildren<FishHpUI>();
        Sensor = GetComponentInChildren<FishSensor>();

        Data = JsonManager.Ins.GetDataByKey<FishData>(_fishId, x => x.fishId);

        if (Data == null)
        {
            Debug.LogError($"FishData 없음: fishId = {_fishId}", this);
            enabled = false;
            return;
        }

        if (Controller == null)
        {
            Debug.LogError("FishController 없음", this);
            enabled = false;
            return;
        }

        if (Stats == null)
        {
            Debug.LogError("FishStats 없음", this);
            enabled = false;
            return;
        }

        if (Sensor == null)
        {
            Debug.LogError("FishSensor 없음", this);
            enabled = false;
            return;
        }

        Sensor.Init(this);
        Stats.Init(this);
        Controller.Init(this);
        HpUI?.Init(Stats);
    }
}
