using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private Transform _weaponModel;
    [SerializeField] private Weapon _weapon;

    [Header("공격 관련")]
    [SerializeField] private float _attackDistance = 1.2f;
    [SerializeField] private float _attackSpeed = 15f;
    [SerializeField] private float _attackCooldown = 0.5f;

    [SerializeField] private float _originY_Right = -0.74f;
    [SerializeField] private float _originY_Left = 0.1f;

    private PlayerRoot _owner;

    private Vector3 _weaponLocalOrigin;
    private bool _isAttacking = false;

    private float _lastFacingX = 1f;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        if (_weaponTransform != null)
            _weaponLocalOrigin = _weaponTransform.localPosition;

        ApplyWeaponSideAndMirror(forceSnap: true);
    }

    // 키보드 공격 임시테스트용
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnAttack();
        }
    }

    private void LateUpdate()
    {
        if (_owner == null) return;
        if (_isAttacking) return;

        ApplyWeaponSideAndMirror(forceSnap: true);
    }

    public void OnAttack()
    {
        if (_owner == null || _weaponTransform == null) return;
        if (_isAttacking) return;

        StartCoroutine(C_Attack());
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

        _weapon?.BeginAttack();

        while (Vector3.Distance(_weaponTransform.localPosition, targetPos) > 0.01f)
        {
            _weaponTransform.localPosition = Vector3.MoveTowards(
                _weaponTransform.localPosition,
                targetPos,
                _attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        _weapon?.EndAttack();

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
