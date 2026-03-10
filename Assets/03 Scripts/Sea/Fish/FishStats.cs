using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 2;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<int, int> OnDamaged;

    private FishRoot _owner;

    public void Init(FishRoot owner)
    {
        _owner = owner;
        CurrentHp = _maxHp;
        IsDead = false;
    }

    public void TakeDamage(DamageInfo info)
    {
        TakeDamage(info.Damage);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);

        OnDamaged?.Invoke(CurrentHp, _maxHp);

        if (CurrentHp <= 0) Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        //playerInventory.AddFish(fishID);

        Debug.Log("물고기 사망");

        if (_owner != null && _owner.Controller != null)
            _owner.Controller.enabled = false;
    }
}
