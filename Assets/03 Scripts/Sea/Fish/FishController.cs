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

    private FishState _currentState = FishState.Patrol;
    private Vector3 _startPosition;
    private int _direction = 1;
    private Transform _target;
    private float _cooldownTimer = 0f;

    private void Awake()
    {
        _direction = Random.Range(0, 2) == 0 ? 1 : -1;
        _startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _target = playerObj.transform;
    }

    private void Update()
    {
        if (_target == null) return;

        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _target.position);

        UpdateState(distanceToPlayer);

        switch (_currentState)
        {
            case FishState.Patrol:
                Patrol();
                break;
            case FishState.Chase:
                Chase(distanceToPlayer);
                break;
            case FishState.AttackDelay:
                break;
        }
    }

    private void UpdateState(float distance)
    {
        if (_cooldownTimer > 0)
        {
            _currentState = FishState.AttackDelay;
            return;
        }

        if (distance <= _attackRange)
        {
            OnAttack();
            return;
        }

        if (distance <= _detectionRange)
        {
            _currentState = FishState.Chase;
        }
        else
        {
            _currentState = FishState.Patrol;
        }
    }

    private void Patrol()
    {
        transform.Translate(Vector3.right * _direction * _moveSpeed * Time.deltaTime);

        float distanceFromStart = transform.position.x - _startPosition.x;
        if (Mathf.Abs(distanceFromStart) > _moveRangeX)
        {
            _direction *= -1;
            float clampedX = _startPosition.x + (Mathf.Sign(distanceFromStart) * _moveRangeX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }

        UpdateFacing(Vector3.right * _direction);
    }

    private void Chase(float distance)
    {
        Vector3 directionToPlayer = (_target.position - transform.position).normalized;
        transform.position += directionToPlayer * _chaseSpeed * Time.deltaTime;

        UpdateFacing(directionToPlayer);
    }

    private void OnAttack()
    {
        _cooldownTimer = _attackCooldown;
        _currentState = FishState.AttackDelay;
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        if (moveDir.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDir.x < 0) transform.localScale = new Vector3(1, 1, 1);
    }
}
