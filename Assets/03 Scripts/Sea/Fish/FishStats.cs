using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour, IDamageable
{
    private float _maxHp;
    private float _attackDamage;

    private FishData _data;
    private FishRoot _owner;

    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnDamaged;
    public event Action<int> OnDead;

    public void Init(FishRoot owner, FishData data)
    {
        _owner = owner;
        _data = data;

        _maxHp = data.hp;
        _attackDamage = data.attackDamage;

        CurrentHp = _maxHp;
        IsDead = false;
    }

    public void TakeDamage(DamageInfo info)
    {
        TakeDamage(info.Damage);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);

        OnDamaged?.Invoke(CurrentHp, _maxHp);

        if (CurrentHp <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        OnDead?.Invoke(_owner.fishId);

        Debug.Log($"물고기 사망: {_data.name}");

        if (_owner?.Controller != null)
            _owner.Controller.enabled = false;

        Destroy(gameObject);
    }
}
