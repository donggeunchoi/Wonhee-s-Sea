using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _sr;

    private float _maxSpeed = 3f;
    private float _acceleration = 10f;
    private float _deadZone = 0.15f;

    private PlayerRoot _owner;
    private Rigidbody2D _rb;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _rb = owner.Rb;

        if (_anim == null) _anim = owner.Anim;
        if (_sr == null) _sr = GetComponentInChildren<SpriteRenderer>();

        SetupRigidbody();
    }

    private void SetupRigidbody()
    {
        if (_rb == null) return;

        _rb.gravityScale = 0f;
        _rb.drag = 3f;
        _rb.angularDrag = 3f;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (_owner == null || _owner.Input == null || _rb == null) return;

        Vector2 input = GetMoveInput();

        Move(input);
        UpdateAnimation(input);
    }

    private Vector2 GetMoveInput()
    {
        Vector2 raw = _owner.Input.Move;

        if (raw.magnitude < _deadZone) return Vector2.zero;
        return raw.sqrMagnitude > 1f ? raw.normalized : raw;
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

    private void UpdateAnimation(Vector2 input)
    {
        if (_anim == null) return;

        bool isMove = input.sqrMagnitude > 0.0001f;
        _anim.SetBool("isMove", isMove);

        if (!isMove)
        {
            _anim.SetInteger("moveDir", 0);
            return;
        }

        Vector2 dir = input.normalized;

        int moveDir;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            moveDir = 1;

            if (_sr != null)
            {
                if (dir.x < 0f) _sr.flipX = true;
                else if (dir.x > 0f) _sr.flipX = false;
            }
        }
        else
        {
            moveDir = (dir.y > 0f) ? 2 : 3;
        }

        _anim.SetInteger("moveDir", moveDir);
    }
}

