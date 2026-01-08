using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 1f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _deadZone = 0.15f;


    Rigidbody2D _rb;
    Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.drag = 3f;
        _rb.angularDrag = 3f;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void SetMoveInput(Vector2 input)
    {
        if (input.magnitude < _deadZone)
            _moveInput = Vector2.zero;
        else
            _moveInput = input;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector2 targetVelocity = _moveInput * _maxSpeed;

        _rb.velocity = Vector2.Lerp(
            _rb.velocity,
            targetVelocity,
            _acceleration * Time.fixedDeltaTime
        );
    }

    private void Rotate()
    {
        if (_rb.velocity.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle - 90f);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            _rotationSpeed * Time.fixedDeltaTime
        );
    }
}

