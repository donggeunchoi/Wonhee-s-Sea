using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonWeapon : BaseWeapon
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _speed = 16f;
    [SerializeField] private int _damage = 3;
    [SerializeField] private float _cooldown = 0.45f;

    private float _cdTimer;

    private void Update()
    {
        if (_cdTimer > 0) _cdTimer -= Time.deltaTime;
    }

    public override void Fire(Vector2 direction)
    {
        if (_cdTimer > 0 || _projectilePrefab == null || _firePoint == null) return;

        direction = direction.normalized;
        var proj = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        proj.Init(_owner, direction, _damage, _speed);

        _cdTimer = _cooldown;
    }
}
