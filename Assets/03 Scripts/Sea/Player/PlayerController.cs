using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Mode { Dungeon, Town }

    [Header("옵션")]
    [SerializeField] private Mode _mode = Mode.Dungeon;
    [SerializeField] private bool _useRotation = true;
    [SerializeField] private float _maxSpeed = 3f;

    [Header("이미지")]
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _sr;

    private float _acceleration = 10f;
    private float _deadZone = 0.15f;
    private float _rotationSpeed = 10f;
    private float _rotationOffset = -90f;
    private int _lastDir = 0;

    private PlayerRoot _owner;
    private Rigidbody2D _rb;


    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _rb = owner.Rb;
        if (_anim == null) _anim = owner.Anim;

        SetupRigidbody();
    }

    private void SetupRigidbody()
    {
        if (_rb == null) return;
        _rb.gravityScale = 0f;
        _rb.drag = 3f;
        _rb.angularDrag = 3f;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void LateUpdate()
    {
        if (_mode != Mode.Town) return;
        if (_sr == null) return;

        Vector2 input = GetMoveInput();
        if (input.x > 0.1f) _sr.flipX = false;
        else if (input.x < -0.1f) _sr.flipX = true;
    }

    private void FixedUpdate()
    {
        if (_owner == null || _owner.Input == null || _rb == null) return;

        Vector2 input = GetMoveInput();
        Move(input);

        if (_useRotation && _mode == Mode.Dungeon)
            RotateByVelocity();

        UpdateAnimation(input);
    }

    private Vector2 GetMoveInput()
    {
        Vector2 raw = _owner.Input.Move;
        return (raw.magnitude < _deadZone) ? Vector2.zero : raw;
    }

    private void Move(Vector2 input)
    {
        Vector2 targetVelocity = input * _maxSpeed;
        _rb.velocity = Vector2.Lerp(_rb.velocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
    }

    private void RotateByVelocity()
    {
        if (_rb.velocity.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle + _rotationOffset);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation(Vector2 input)
    {
        if (_anim == null) return;

        if (_mode == Mode.Dungeon)
        {
            bool isMove = _rb.velocity.sqrMagnitude > 0.01f;
            _anim.SetBool("isMove", isMove);
            return;
        }

        bool isMoveTown = input.sqrMagnitude > 0.01f;
        _anim.SetBool("isMove", isMoveTown);

        if (!isMoveTown)
        {
            _anim.SetInteger("moveDir", _lastDir);
            return;
        }

        float ax = Mathf.Abs(input.x);
        float ay = Mathf.Abs(input.y);

        int dir = (ax > ay) ? 1 : (input.y >= 0 ? 2 : 3);
        _lastDir = dir;
        _anim.SetInteger("moveDir", dir);
    }
}

