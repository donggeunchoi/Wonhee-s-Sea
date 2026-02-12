using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Melee, Ranged }

public class PlayerAttackController : MonoBehaviour
{
    [Header("무기 타입")]
    [SerializeField] private WeaponType _weaponType = WeaponType.Melee;

    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private Transform _weaponModel;

    [Header("근접")]
    [SerializeField] private Weapon _meleeWeapon;
    [SerializeField] private float _attackDistance = 1.2f;
    [SerializeField] private float _attackSpeed = 15f;
    [SerializeField] private float _attackCooldown = 0.5f;

    [Header("원거리")]
    [SerializeField] private SpearProjectile _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _projectileSpeed = 12f;
    [SerializeField] private int _projectileDamage = 1;
    [SerializeField] private float _rangedCooldown = 0.25f;

    [Header("공격 버튼")]
    [SerializeField] private GameObject _attackButtonObj;
    [SerializeField] private GameObject _aimJoystickObj;

    private float _originY_Right = -0.74f;
    private float _originY_Left = 0.1f;

    private PlayerRoot _owner;

    private Vector3 _weaponLocalOrigin;
    private bool _isAttacking = false;
    private float _lastFacingX = 1f;

    private float _rangedCdTimer;

    public WeaponType CurrentWeaponType => _weaponType;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        if (_weaponTransform != null)
            _weaponLocalOrigin = _weaponTransform.localPosition;

        ApplyWeaponSideAndMirror(forceSnap: true);
        ApplyModeUI();
    }

    // 키보드 공격 임시테스트용
    private void Update()
    {
        if (_rangedCdTimer > 0f) _rangedCdTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_weaponType == WeaponType.Melee)
                OnAttack();
        }
    }

    private void LateUpdate()
    {
        if (_owner == null) return;
        if (_isAttacking) return;

        ApplyWeaponSideAndMirror(forceSnap: true);
    }

    public void SetWeaponType(WeaponType type)
    {
        if (_weaponType == type) return;

        _weaponType = type;

        if (_isAttacking)
        {
            StopAllCoroutines();
            _isAttacking = false;

            if (_weaponTransform != null)
                _weaponTransform.localPosition = _weaponLocalOrigin;

            _meleeWeapon?.EndAttack();
        }

        ApplyModeUI();
        ApplyWeaponSideAndMirror(forceSnap: true);
    }

    private void ApplyModeUI()
    {
        bool ranged = _weaponType == WeaponType.Ranged;

        if (_attackButtonObj != null) _attackButtonObj.SetActive(!ranged);
        if (_aimJoystickObj != null) _aimJoystickObj.SetActive(ranged);
    }

    public void OnAttack()
    {
        if (_weaponType != WeaponType.Melee) return;
        if (_owner == null || _weaponTransform == null) return;
        if (_isAttacking) return;

        StartCoroutine(C_Attack());
    }

    public void FireRanged(Vector2 shootDir)
    {
        if (_weaponType != WeaponType.Ranged) return;
        if (_rangedCdTimer > 0f) return;
        if (_projectilePrefab == null) return;
        if (shootDir.sqrMagnitude < 0.0001f) return;

        shootDir = shootDir.normalized;

        Transform fp = _firePoint != null ? _firePoint : _weaponTransform;
        if (fp == null) return;

        var proj = Instantiate(_projectilePrefab, fp.position, Quaternion.identity);
        proj.Init(_owner.gameObject, shootDir, _projectileDamage, _projectileSpeed);

        _rangedCdTimer = _rangedCooldown;
    }

    private void ApplyWeaponSideAndMirror(bool forceSnap)
    {
        Vector2 facing = (_owner != null && _owner.Input != null) ? _owner.Input.FacingDir : Vector2.right;
        if (Mathf.Abs(facing.x) > 0.01f) _lastFacingX = Mathf.Sign(facing.x);

        bool flip = (_owner != null) ? _owner.Controller.IsFlipY : (_lastFacingX < 0f);

        if (_weaponTransform != null)
        {
            Vector3 origin = _weaponTransform.localPosition;
            origin.y = flip ? _originY_Left : _originY_Right;

            _weaponLocalOrigin = origin;

            if (forceSnap)
                _weaponTransform.localPosition = _weaponLocalOrigin;
        }

        if (_weaponModel != null)
        {
            Vector3 s = _weaponModel.localScale;
            s.y = flip ? -Mathf.Abs(s.y) : Mathf.Abs(s.y);
            _weaponModel.localScale = s;
        }
    }

    private IEnumerator C_Attack()
    {
        _isAttacking = true;
        _owner.Controller?.Freeze(0.6f);

        ApplyWeaponSideAndMirror(forceSnap: true);

        Vector2 facing = (_owner != null && _owner.Input != null) ? _owner.Input.FacingDir : Vector2.right;

        Transform space = (_weaponTransform.parent != null) ? _weaponTransform.parent : _owner.transform;
        Vector3 localDir = space.InverseTransformDirection((Vector3)facing).normalized;

        Vector3 origin = _weaponLocalOrigin;
        Vector3 targetPos = origin + localDir * _attackDistance;

        _meleeWeapon?.BeginAttack();

        while (Vector3.Distance(_weaponTransform.localPosition, targetPos) > 0.01f)
        {
            _weaponTransform.localPosition = Vector3.MoveTowards(
                _weaponTransform.localPosition,
                targetPos,
                _attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        _meleeWeapon?.EndAttack();

        while (Vector3.Distance(_weaponTransform.localPosition, origin) > 0.01f)
        {
            _weaponTransform.localPosition = Vector3.MoveTowards(
                _weaponTransform.localPosition,
                origin,
                _attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        _weaponTransform.localPosition = origin;

        yield return new WaitForSeconds(_attackCooldown);
        _isAttacking = false;
    }
}
