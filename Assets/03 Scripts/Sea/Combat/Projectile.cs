using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _lifeTime = 3f;

    private GameObject _owner;
    private Vector2 _dir;
    private float _speed;
    private int _damage;
    private bool _isHit;

    public void Init(GameObject owner, Vector2 dir, int damage, float speed)
    {
        _owner = owner;
        _dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        _damage = damage;
        _speed = speed;
        _isHit = false;

        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHit) return;

        if (((1 << other.gameObject.layer) & _targetMask.value) == 0) return;

        if (_owner != null && (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)))
            return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null || dmg.IsDead) return;

        _isHit = true;

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)other.transform.position - hitPoint).normalized;

        dmg.TakeDamage(new DamageInfo(_owner ?? gameObject, _damage, hitPoint, normal));
        Destroy(gameObject);
    }
}