using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHp = 5f;
    [SerializeField] private float _invincibleTime = 0.5f;

    public float MaxHp => _maxHp;
    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvincible => _invTimer > 0f;

    private float _invTimer;

    public event Action<float, float> OnHpChanged;

    private void Awake()
    {
        CurrentHp = _maxHp;
        IsDead = false;
        OnHpChanged?.Invoke(CurrentHp, _maxHp);
    }

    private void Update()
    {
        if (_invTimer > 0f)
            _invTimer -= Time.deltaTime;
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
        OnHpChanged?.Invoke(CurrentHp, _maxHp);

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
