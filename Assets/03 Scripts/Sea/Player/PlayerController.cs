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

    Rigidbody2D _rb;
    Vector2 _moveInput;
    private int _lastDir = 0; // 기본값 서있기

    [Header( "이미지" )]
    [SerializeField]
    private SpriteRenderer sr;

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

    public void SetMoveInput(Vector2 input)
    {
        if (input.magnitude < _deadZone)
            _moveInput = Vector2.zero;
        else
            _moveInput = input;
    }

    private void LateUpdate()
    {
        if (sr == null) return;
        if (_moveInput.x > 0.1f) sr.flipX = false;
        else if (_moveInput.x < -0.1f) sr.flipX = true;
    }

    private void FixedUpdate()
    {
        if (_owner == null || _owner.Input == null) return;

        Vector2 raw = _owner.Input.Move;
        Vector2 input = raw.magnitude < _deadZone ? Vector2.zero : raw;

        Move(input);

        if (_useRotation)
            Rotate();

        //작동하는 방식이 달라서 일단 주석처리
        //UpdateAnim();

        UpdateTownAnim();
    }

    private void Move(Vector2 input)
    {
        //Vector2 targetVelocity = input * _maxSpeed;

        if (_anim == null) return;

        bool isMove = _rb.velocity.sqrMagnitude > 0.01f;
        _anim.SetBool("isMove", isMove);
    }

    public void UpdateTownAnim()
    {
        if (_anim == null) return;

        bool isMove = _moveInput.sqrMagnitude > 0.01f;
        _anim.SetBool("isMove", isMove);

        if (!isMove)
        {
            _anim.SetInteger("moveDir", _lastDir); // 멈춰도 마지막 방향 유지
            return;
        }

        float ax = Mathf.Abs(_moveInput.x);
        float ay = Mathf.Abs(_moveInput.y);

        int dir;
        if (ax > ay) dir = 1;                 // Side
        else dir = (_moveInput.y >= 0) ? 2 : 3; // Up/Down

        _lastDir = dir;
        _anim.SetInteger("moveDir", dir);
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

    private void UpdateAnim()
    {
        if (_anim == null) return;

        bool isMove = _rb.velocity.sqrMagnitude > 0.01f;
        _anim.SetBool("isMove", isMove);
    }
}

