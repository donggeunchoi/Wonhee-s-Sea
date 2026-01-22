using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _deadZone = 0.15f;

    [Header("옵션")]
    [SerializeField] private bool _useRotation = true;

    [Header("애니메이션")]
    [SerializeField] private Animator _anim;

    private PlayerRoot _owner;
    private Rigidbody2D _rb;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _rb = owner.Rb;

        if (_anim == null) _anim = owner.Anim;

        _rb.gravityScale = 0f;
        _rb.drag = 3f;
        _rb.angularDrag = 3f;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        if (_owner == null || _owner.Input == null) return;

        Vector2 raw = _owner.Input.Move;
        Vector2 input = raw.magnitude < _deadZone ? Vector2.zero : raw;

        Move(input);

        if (_useRotation)
            Rotate();

        UpdateAnim();
    }

    private void Move(Vector2 input)
    {
        Vector2 targetVelocity = input * _maxSpeed;

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

    private void UpdateAnim()
    {
        if (_anim == null) return;

        bool isMove = _rb.velocity.sqrMagnitude > 0.01f;
        _anim.SetBool("isMove", isMove);
    }
}

