using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerWeapon : BaseWeapon
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private Collider2D _hitCollider;
    [SerializeField] private int _damage = 4;        // ← Inspector에서 조정 가능

    private readonly HashSet<IDamageable> _hitSet = new();
    private bool _isActive;

    private void Awake()
    {
        if (_hitCollider == null) _hitCollider = GetComponent<Collider2D>();
        if (_hitCollider != null)
        {
            _hitCollider.isTrigger = true;
            _hitCollider.enabled = false;
        }
    }

    public override void BeginAttack()
    {
        _hitSet.Clear();
        _isActive = true;
        if (_hitCollider != null) _hitCollider.enabled = true;
    }

    public override void EndAttack()
    {
        _isActive = false;
        if (_hitCollider != null) _hitCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        if (((1 << other.gameObject.layer) & _targetMask.value) == 0) return;
        if (_owner != null && (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform))) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead || _hitSet.Contains(dmg)) return;

        _hitSet.Add(dmg);

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)other.transform.position - hitPoint).normalized;

        dmg.TakeDamage(new DamageInfo(_owner ?? gameObject, _damage, hitPoint, normal));
    }
}
