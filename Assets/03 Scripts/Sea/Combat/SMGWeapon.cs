using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGWeapon : BaseWeapon
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.12f;
    [SerializeField] private int _damage = 1;

    private float _nextFireTime;

    public override void Fire(Vector2 direction)
    {
        if (Time.time < _nextFireTime || _projectilePrefab == null || _firePoint == null) return;

        var projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        projectile.Init(_owner, direction, _damage, 16f);
        _nextFireTime = Time.time + _fireRate;
    }
}
