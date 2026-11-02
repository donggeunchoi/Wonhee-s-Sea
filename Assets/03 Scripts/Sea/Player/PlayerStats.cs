using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private float _baseMaxHp = 500f;
    [SerializeField] private float _invincibleTime = 0.5f;

    private PlayerRoot _owner;
    private float _invTimer;
    private float _lastMaxHp;

    public float MaxHp
    {
        get
        {
            float bonus = 0f;

            if (_owner != null && _owner.Equipment != null)
                bonus += _owner.Equipment.BonusHp;

            return _baseMaxHp + bonus;
        }
    }

    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvincible => _invTimer > 0f;

    public event Action<float, float> OnHpChanged;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        IsDead = false;
        _invTimer = 0f;

        _lastMaxHp = MaxHp;
        CurrentHp = _lastMaxHp;

        OnHpChanged?.Invoke(CurrentHp, _lastMaxHp);
    }

    private void Update()
    {
        if (_invTimer > 0f)
            _invTimer -= Time.deltaTime;
    }

    public void RefreshByEquipment()
    {
        float newMaxHp = MaxHp;
        float delta = newMaxHp - _lastMaxHp;

        CurrentHp += delta;
        CurrentHp = Mathf.Clamp(CurrentHp, 0f, newMaxHp);

        _lastMaxHp = newMaxHp;

        OnHpChanged?.Invoke(CurrentHp, newMaxHp);
    }

    public void TakeDamage(DamageInfo info)
    {
        TakeDamage(info.Damage);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (IsInvincible) return;

        ApplyDamage(amount, ignoreInvincible: false);
    }

    public void TakeDamageIgnoreInvincible(float amount)
    {
        if (IsDead) return;
        ApplyDamage(amount, ignoreInvincible: true);
    }

    private void ApplyDamage(float amount, bool ignoreInvincible)
    {
        if (amount <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);

        if (!ignoreInvincible)
            _invTimer = _invincibleTime;

        if (CurrentHp <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        var inv = GetComponent<PlayerInventory>();
        if (inv != null)
            inv.LoseFishPercent(0.9f);

        Debug.Log("플레이어 사망");
    }
}
