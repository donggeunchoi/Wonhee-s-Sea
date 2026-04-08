using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour, IDamageable
{
    private FishRoot _owner;

    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnDamaged;
    public event Action<int> OnDead;

    public void Init(FishRoot owner)
    {
        _owner = owner;

        if (_owner == null || _owner.Data == null)
        {
            Debug.LogError("FishStats Init 실패: owner 또는 data가 null", this);
            enabled = false;
            return;
        }

        CurrentHp = _owner.Data.hp;
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

        OnDamaged?.Invoke(CurrentHp, _owner.Data.hp);

        if (CurrentHp <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        OnDead?.Invoke(_owner.FishId);

        if (_owner.Controller != null)
            _owner.Controller.enabled = false;

        Destroy(gameObject);
    }
}
