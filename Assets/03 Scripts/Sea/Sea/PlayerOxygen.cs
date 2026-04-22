using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float _maxOxygen = 60f;
    [SerializeField] private float _drainInterval = 2.0f;
    [SerializeField] private float _drainAmountPerTick = 3f;
    [SerializeField] private int _suffocationDamage = 1;
    [SerializeField] private float _damageInterval = 1f;

    public float CurrentOxygen { get; private set; }
    public float MaxOxygen => _maxOxygen;

    public bool IsUnderwater { get; set; } = true;

    public event Action<float, float> OnOxygenChanged;

    private PlayerStats _stats;

    private float _drainTimer;
    private float _damageTimer;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        CurrentOxygen = _maxOxygen;
        OnOxygenChanged?.Invoke(CurrentOxygen, _maxOxygen);
    }

    private void Update()
    {
        if (_stats != null && _stats.IsDead) return;

        float dt = Time.deltaTime;

        if (IsUnderwater)
        {
            TickDrain(dt);
            SuffocationTick(dt);
        }
        else
        {
            _drainTimer = 0f;
            _damageTimer = 0f;
        }
    }

    private void TickDrain(float dt)
    {
        if (_drainInterval <= 0f || _drainAmountPerTick <= 0f) return;
        if (CurrentOxygen <= 0f) return;

        _drainTimer += dt;

        while (_drainTimer >= _drainInterval && CurrentOxygen > 0f)
        {
            _drainTimer -= _drainInterval;

            float prev = CurrentOxygen;
            CurrentOxygen = Mathf.Max(0f, CurrentOxygen - _drainAmountPerTick);

            if (!Mathf.Approximately(prev, CurrentOxygen))
                OnOxygenChanged?.Invoke(CurrentOxygen, _maxOxygen);
        }
    }

    private void SuffocationTick(float dt)
    {
        if (CurrentOxygen > 0f) { _damageTimer = 0f; return; }
        if (_stats == null || _stats.IsDead) return;

        _damageTimer += dt;
        if (_damageTimer >= _damageInterval)
        {
            _damageTimer -= _damageInterval;
            _stats.TakeDamageIgnoreInvincible(_suffocationDamage);
        }
    }

    public void AddOxygen(float amount)
    {
        if (amount <= 0f) return;

        float prev = CurrentOxygen;
        CurrentOxygen = Mathf.Min(_maxOxygen, CurrentOxygen + amount);

        if (!Mathf.Approximately(prev, CurrentOxygen))
            OnOxygenChanged?.Invoke(CurrentOxygen, _maxOxygen);
    }
}
