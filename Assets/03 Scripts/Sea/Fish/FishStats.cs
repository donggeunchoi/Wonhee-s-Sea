using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStats : MonoBehaviour
{
    [SerializeField] private int _maxHp = 2;
    [SerializeField] private float _destroyDelay = 1.0f;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private Animator _animator;
    private FishController _controller;
    private Collider2D _collider;

    private void Awake()
    {
        CurrentHp = _maxHp;
        IsDead = false;

        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<FishController>();
        _collider = GetComponent<Collider2D>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (_controller != null)
            _controller.enabled = false;

        if (_collider != null)
            _collider.enabled = false;

        if (_animator != null)
            _animator.SetTrigger("isDead");

        Destroy(gameObject, _destroyDelay);
    }
}
