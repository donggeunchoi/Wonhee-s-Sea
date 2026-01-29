using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    private enum FishState { Patrol, Chase, AttackDelay }

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 0.5f;
    [SerializeField] private float _chaseSpeed = 0.7f;
    [SerializeField] private float _moveRangeX = 5f;

    [Header("공격 설정")]
    [SerializeField] private float _detectionRange = 3f;
    [SerializeField] private float _attackRange = 0.5f;
    [SerializeField] private float _attackCooldown = 2.0f;
    [SerializeField] private int _attackDamage = 1;

    private FishState _state = FishState.Patrol;

    private Vector3 _startPosition;
    private int _direction = 1;
    private float _cooldownTimer = 0f;

    private Transform _targetTr;
    private PlayerStats _targetStats;

    private FishRoot _owner;

    public void Init(FishRoot owner)
    {
        _owner = owner;

        _direction = Random.Range(0, 2) == 0 ? 1 : -1;
        _startPosition = transform.position;
    }

    public void BindTarget(Transform playerTr, PlayerStats playerStats)
    {
        _targetTr = playerTr;
        _targetStats = playerStats;
    }

    private void Update()
    {
        if (_targetTr == null) { Patrol(); return; }

        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, _targetTr.position);

        if (_cooldownTimer > 0f)
        {
            _state = FishState.AttackDelay;
            UpdateFacing((_targetTr.position - transform.position).normalized);
            return;
        }

        if (distance <= _attackRange)
        {
            Attack();
            return;
        }

        _state = (distance <= _detectionRange) ? FishState.Chase : FishState.Patrol;

        if (_state == FishState.Chase) Chase();
        else Patrol();
    }

    private void Patrol()
    {
        _state = FishState.Patrol;

        transform.Translate(Vector3.right * _direction * _moveSpeed * Time.deltaTime);

        float dx = transform.position.x - _startPosition.x;
        if (Mathf.Abs(dx) > _moveRangeX)
        {
            _direction *= -1;
            float clampedX = _startPosition.x + (Mathf.Sign(dx) * _moveRangeX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }

        UpdateFacing(Vector3.right * _direction);
    }

    private void Chase()
    {
        Vector3 dir = (_targetTr.position - transform.position).normalized;
        transform.position += dir * _chaseSpeed * Time.deltaTime;
        UpdateFacing(dir);
    }

    private void Attack()
    {
        _cooldownTimer = _attackCooldown;
        _state = FishState.AttackDelay;

        if (_targetStats == null)
        {
            return;
        }

        Debug.Log($"물고기가 공격함 데미지:{_attackDamage}");
        _targetStats.TakeDamage(_attackDamage);
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        if (moveDir.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDir.x < 0) transform.localScale = new Vector3(1, 1, 1);
    }
}
