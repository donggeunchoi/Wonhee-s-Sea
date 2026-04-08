using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    private enum FishState { Patrol, Chase, AttackDelay }

    [SerializeField] private float _moveRangeX = 5f;
    [SerializeField] private Transform _visual;

    private FishState _state = FishState.Patrol;

    private Vector3 _startPosition;
    private int _direction = 1;
    private float _cooldownTimer = 0f;

    private Transform _targetTr;
    private IDamageable _targetDamageable;
    private FishRoot _owner;

    public void Init(FishRoot owner)
    {
        _owner = owner;

        if (_owner == null || _owner.Data == null)
        {
            Debug.LogError("FishController Init 실패: owner 또는 data가 null", this);
            enabled = false;
            return;
        }

        _direction = Random.Range(0, 2) == 0 ? 1 : -1;
        _startPosition = transform.position;
    }

    public void BindTarget(Transform targetTr)
    {
        _targetTr = targetTr;
        _targetDamageable = targetTr != null ? targetTr.GetComponentInParent<IDamageable>() : null;
    }

    private void Update()
    {
        if (_owner == null || _owner.Data == null)
            return;

        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (_targetTr == null)
        {
            Patrol();
            return;
        }

        float distance = Vector2.Distance(transform.position, _targetTr.position);

        if (!_owner.Data.isAttack)
        {
            Patrol();
            return;
        }

        if (_cooldownTimer > 0f)
        {
            _state = FishState.AttackDelay;
            UpdateFacing((_targetTr.position - transform.position).normalized);
            return;
        }

        if (distance <= _owner.Data.attackRange/5f) // 데이터 값이 너무 높아서 추후 수정
        {
            Attack();
            return;
        }

        _state = distance <= _owner.Data.detectionRange*3f ? FishState.Chase : FishState.Patrol; // 데이터값이 너무 낮아서 추후 수정

        if (_state == FishState.Chase)
            Chase();
        else
            Patrol();
    }

    private void Patrol()
    {
        _state = FishState.Patrol;

        transform.Translate(Vector3.right * _direction * _owner.Data.moveSpeed / 3f * Time.deltaTime); // 데이터 값이 너무 높아서 추후 수정

        float dx = transform.position.x - _startPosition.x;

        if (Mathf.Abs(dx) > _moveRangeX)
        {
            _direction *= -1;

            float clampedX = _startPosition.x + Mathf.Sign(dx) * _moveRangeX;
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }

        UpdateFacing(Vector3.right * _direction);
    }

    private void Chase()
    {
        Vector3 dir = (_targetTr.position - transform.position).normalized;
        transform.position += dir * (_owner.Data.moveSpeed * 1.5f / 3f) * Time.deltaTime; // 데이터 값이 너무 높아서 추후 수정
        UpdateFacing(dir);
    }

    private void Attack()
    {
        if (_owner.Data.attackSpeed <= 0f)
            return;

        _cooldownTimer = 10f / _owner.Data.attackSpeed; // 데이터 값이 너무 높아서 추후 수정
        _state = FishState.AttackDelay;

        if (_targetDamageable == null || _targetDamageable.IsDead)
            return;

        Vector2 dir = (_targetTr.position - transform.position).normalized;

        _targetDamageable.TakeDamage(
            new DamageInfo(gameObject, _owner.Data.attackDamage, _targetTr.position, dir)
        );
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        if (_visual == null) return;

        Vector3 scale = _visual.localScale;
        float absX = Mathf.Abs(scale.x);

        if (moveDir.x > 0f)
            scale.x = -absX;
        else if (moveDir.x < 0f)
            scale.x = absX;

        _visual.localScale = scale;
    }
}
