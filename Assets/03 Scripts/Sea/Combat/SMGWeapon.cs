using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGWeapon : BaseWeapon
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.12f;
    [SerializeField] private int _damage = 1;           // 따발총 데미지

    private float _fireTimer;
    private bool _isFiring;

    public override void StartFiring()
    {
        _isFiring = true;
    }

    public override void StopFiring()
    {
        _isFiring = false;
    }

    private void Update()
    {
        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;

        // 연사 중이고, 조준 중이면 발사
        if (_isFiring && _owner != null && _playerRoot?.Input?.IsAiming == true && _fireTimer <= 0f)
        {
            Fire(_playerRoot.Input.FacingDir);
            _fireTimer = _fireRate;
        }
    }

    public override void Fire(Vector2 direction)
    {
        if (_projectilePrefab == null || _firePoint == null) return;

        direction = direction.normalized;
        var proj = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        proj.Init(_owner, direction, _damage, 16f);
    }
}
