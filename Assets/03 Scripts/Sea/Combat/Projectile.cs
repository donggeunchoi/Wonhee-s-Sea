using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _lifeTime = 3f;

    private GameObject _owner;
    private Vector2 _dir;
    private float _speed;
    private int _damage;

    public void Init(GameObject owner, Vector2 dir, int damage, float speed)
    {
        _owner = owner;
        _dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        _damage = damage;
        _speed = speed;

        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _targetMask.value) == 0) return;

        if (_owner != null && (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)))
            return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)other.transform.position - hitPoint).normalized;

        dmg.TakeDamage(new DamageInfo(_owner ?? gameObject, _damage, hitPoint, normal));
        Destroy(gameObject);
    }
}