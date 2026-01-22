using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private Transform _weaponTransform;

    [Header("공격 관련")]
    [SerializeField] private float _attackDistance = 1.2f;
    [SerializeField] private float _attackSpeed = 15f;
    [SerializeField] private float _attackCooldown = 0.5f;

    private PlayerRoot _owner;

    private Vector3 _weaponLocalOrigin;
    private bool _isAttacking = false;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        if (_weaponTransform != null)
            _weaponLocalOrigin = _weaponTransform.localPosition;
    }

    public void OnAttack()
    {
        if (_owner == null || _weaponTransform == null) return;
        if (_isAttacking) return;

        StartCoroutine(C_Attack());
    }

    private IEnumerator C_Attack()
    {
        _isAttacking = true;

        Vector2 facing = _owner.Input != null ? _owner.Input.FacingDir : Vector2.right;
        Vector3 localDir = _owner.transform.InverseTransformDirection(facing).normalized;

        Vector3 targetPos = _weaponLocalOrigin + localDir * _attackDistance;

        while (Vector3.Distance(_weaponTransform.localPosition, targetPos) > 0.01f)
        {
            _weaponTransform.localPosition = Vector3.MoveTowards(
                _weaponTransform.localPosition,
                targetPos,
                _attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        while (Vector3.Distance(_weaponTransform.localPosition, _weaponLocalOrigin) > 0.01f)
        {
            _weaponTransform.localPosition = Vector3.MoveTowards(
                _weaponTransform.localPosition,
                _weaponLocalOrigin,
                _attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        _weaponTransform.localPosition = _weaponLocalOrigin;

        yield return new WaitForSeconds(_attackCooldown);
        _isAttacking = false;
    }
}
