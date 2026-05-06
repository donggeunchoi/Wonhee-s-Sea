using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float _baseMaxOxygen = 60f;
    [SerializeField] private float _drainInterval = 2.0f;
    [SerializeField] private float _drainAmountPerTick = 3f;
    [SerializeField] private int _suffocationDamage = 1;
    [SerializeField] private float _damageInterval = 1f;

    public float CurrentOxygen { get; private set; }

    public float MaxOxygen
    {
        get
        {
            float bonus = 0f;

            if (_owner != null && _owner.Equipment != null)
                bonus += _owner.Equipment.BonusOxygen;

            return _baseMaxOxygen + bonus;
        }
    }

    public bool IsUnderwater { get; set; } = true;

    public event Action<float, float> OnOxygenChanged;

    private PlayerRoot _owner;
    private PlayerStats _stats;

    private float _drainTimer;
    private float _damageTimer;
    private float _lastMaxOxygen;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _stats = owner != null ? owner.Stats : GetComponent<PlayerStats>();

        _drainTimer = 0f;
        _damageTimer = 0f;

        _lastMaxOxygen = MaxOxygen;
        CurrentOxygen = _lastMaxOxygen;

        OnOxygenChanged?.Invoke(CurrentOxygen, _lastMaxOxygen);
    }

    private void Awake()
    {
        if (_owner == null)
            _owner = GetComponent<PlayerRoot>();

        if (_stats == null)
            _stats = GetComponent<PlayerStats>();
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
                OnOxygenChanged?.Invoke(CurrentOxygen, MaxOxygen);
        }
    }

    private void SuffocationTick(float dt)
    {
        if (CurrentOxygen > 0f)
        {
            _damageTimer = 0f;
            return;
        }

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
        CurrentOxygen = Mathf.Min(MaxOxygen, CurrentOxygen + amount);

        if (!Mathf.Approximately(prev, CurrentOxygen))
            OnOxygenChanged?.Invoke(CurrentOxygen, MaxOxygen);
    }

    public void RefreshByEquipment()
    {
        float newMaxOxygen = MaxOxygen;
        float delta = newMaxOxygen - _lastMaxOxygen;

        CurrentOxygen += delta;
        CurrentOxygen = Mathf.Clamp(CurrentOxygen, 0f, newMaxOxygen);

        _lastMaxOxygen = newMaxOxygen;

        OnOxygenChanged?.Invoke(CurrentOxygen, newMaxOxygen);
    }

    public void RestoreFullOxygen()
    {
        CurrentOxygen = MaxOxygen;
        _lastMaxOxygen = MaxOxygen;
        OnOxygenChanged?.Invoke(CurrentOxygen, MaxOxygen);
    }
}
