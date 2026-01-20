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

    private Vector3 _weaponLocalOrigin;
    private bool _isAttacking = false;

    private void Start()
    {
        if (_weaponTransform != null)
        {
            _weaponLocalOrigin = _weaponTransform.localPosition;
        }
    }

    public void OnAttack()
    {
        if (!_isAttacking && _weaponTransform != null)
        {
            StartCoroutine(C_Attack());
        }
    }

    IEnumerator C_Attack()
    {
        _isAttacking = true;

        Vector3 targetPos = _weaponLocalOrigin + Vector3.right * _attackDistance;

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
