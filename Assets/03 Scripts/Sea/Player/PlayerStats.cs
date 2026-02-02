using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 5;
    [SerializeField] private float _invincibleTime = 0.5f;

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
        if (amount <= 0) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(CurrentHp, _maxHp);

        _invTimer = _invincibleTime;

        if (CurrentHp <= 0) Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log("플레이어 사망");
    }
}
