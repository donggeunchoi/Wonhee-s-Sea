using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour, IDamageable
{
    private FishRoot _owner;
    private PlayerInventory _playerInventory;

    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnDamaged;
    public event Action<int> OnDead;
    public event Action<DamageInfo> OnHit;

    public void Init(FishRoot owner)
    {
        _owner = owner;

        if (_owner == null || _owner.Data == null)
        {
            enabled = false;
            return;
        }

        CurrentHp = _owner.Data.hp;
        IsDead = false;
    }

    public void TakeDamage(DamageInfo info)
    {
        if (IsDead) return;
        if (info.Damage <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - info.Damage);

        OnHit?.Invoke(info);
        OnDamaged?.Invoke(CurrentHp, _owner.Data.hp);

        if (CurrentHp <= 0f)
            Die();
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

        int fishId = _owner.FishId;

        OnDead?.Invoke(fishId);

        _playerInventory = FindObjectOfType<PlayerInventory>();
        _playerInventory.AddFish(fishId);

        if (_owner.Controller != null)
            _owner.Controller.enabled = false;

        Destroy(gameObject);
    }
}
