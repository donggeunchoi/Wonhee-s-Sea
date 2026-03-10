using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 5;
    [SerializeField] private float _invincibleTime = 0.5f;

    public int MaxHp => _maxHp;
    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvincible => _invTimer > 0f;

    private float _invTimer;

    public event Action<int, int> OnHpChanged;

    private void Awake()
    {
        CurrentHp = _maxHp;
        IsDead = false;
        OnHpChanged?.Invoke(CurrentHp, _maxHp);
    }

    private void Update()
    {
        if (_invTimer > 0f) _invTimer -= Time.deltaTime;
    }

    public void TakeDamage(DamageInfo info)
    {
        TakeDamage(info.Damage);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (IsInvincible) return;
        ApplyDamage(amount, ignoreInvincible: false);
    }

    public void TakeDamageIgnoreInvincible(int amount)
    {
        if (IsDead) return;
        ApplyDamage(amount, ignoreInvincible: true);
    }

    private void ApplyDamage(int amount, bool ignoreInvincible)
    {
        if (amount <= 0) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(CurrentHp, _maxHp);

        if (!ignoreInvincible)
            _invTimer = _invincibleTime;

        if (CurrentHp <= 0) Die();
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
