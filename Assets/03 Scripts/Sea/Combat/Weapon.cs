using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private Collider2D _hitCollider;

    private GameObject _owner;
    private bool _active;
    private readonly HashSet<IDamageable> _hitSet = new();

    private int _damage = 1;

    public void SetOwner(GameObject owner) => _owner = owner;

    private void Awake()
    {
        if (_hitCollider == null) _hitCollider = GetComponent<Collider2D>();
        if (_hitCollider != null)
        {
            _hitCollider.isTrigger = true;
            _hitCollider.enabled = false;
        }
    }

    public void BeginAttack()
    {
        _hitSet.Clear();
        _active = true;
        if (_hitCollider != null) _hitCollider.enabled = true;
    }

    public void EndAttack()
    {
        _active = false;
        if (_hitCollider != null) _hitCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active) return;

        if (((1 << other.gameObject.layer) & _targetMask.value) == 0) return;

        if (_owner != null && (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)))
            return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;

        if (_hitSet.Contains(dmg)) return;
        _hitSet.Add(dmg);

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)other.transform.position - hitPoint).normalized;

        dmg.TakeDamage(new DamageInfo(_owner ?? gameObject, _damage, hitPoint, normal));
    }
}
