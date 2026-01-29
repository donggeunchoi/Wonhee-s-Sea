using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 2;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private FishRoot _owner;

    public void Init(FishRoot owner)
    {
        _owner = owner;
        CurrentHp = _maxHp;
        IsDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        Debug.Log($"물고기 데미지 {amount} 받음! (hp {CurrentHp}/{_maxHp})");

        if (CurrentHp <= 0)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log("물고기 다이");

        if (_owner != null && _owner.Controller != null)
            _owner.Controller.enabled = false;
    }
}
