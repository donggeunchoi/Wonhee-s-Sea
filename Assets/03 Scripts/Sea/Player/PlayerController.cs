using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Mode { Town, Dungeon }

    [Header("모드 설정")]
    [SerializeField] private Mode _mode = Mode.Town;

    [Header("이미지")]
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer _sr;

    [Header("이동")]
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private Transform _pivot;

    private float _acceleration = 10f;
    private float _deadZone = 0.15f;
    private float _pivotAngleOffset = 0f;
    private float _maxTurnSpeed = 360f;
    private float _pivotY_Right = 0f;
    private float _pivotY_Left = -0.6f;
    private bool _isFrozen = false;

    public bool IsFlipY { get; private set; } = false;

    private PlayerRoot _owner;
    private Rigidbody2D _rb;
    private Vector2 _lastAimDir = Vector2.right;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _rb = owner.Rb;

        if (_anim == null) _anim = owner.Anim;
        if (_sr == null) _sr = GetComponentInChildren<SpriteRenderer>();

        if (_pivot == null) _pivot = transform;

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

        if (_isFrozen)
        {
            _rb.velocity = Vector2.zero;

            if (_anim != null)
                _anim.SetBool("isMove", false);

            return;
        }

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
        if (_isFrozen)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        float finalSpeed = _maxSpeed;

        if (_owner != null && _owner.Equipment != null)
            finalSpeed += _owner.Equipment.BonusMoveSpeed;

        Vector2 targetVelocity = input * finalSpeed;

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

        if (_mode == Mode.Town)
        {
            UpdateTownAnimation(input, isMove);
        }
        else
        {
            UpdateDungeonAnimation(input);
        }
    }

    private void UpdateTownAnimation(Vector2 input, bool isMove)
    {
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

    private void UpdateDungeonAnimation(Vector2 input)
    {
        if (_pivot == null) return;

        Vector2 dir;
        if (_owner != null && _owner.Input != null && _owner.Input.IsAiming)
        {
            dir = _owner.Input.FacingDir;
            if (dir.sqrMagnitude < 0.0001f) dir = _lastAimDir;
            else _lastAimDir = dir.normalized;
        }
        else
        {
            if (input != Vector2.zero)
                _lastAimDir = input.normalized;

            dir = _lastAimDir;
        }

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + _pivotAngleOffset;
        float currentAngle = _pivot.localEulerAngles.z;
        float nextAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _maxTurnSpeed * Time.fixedDeltaTime);

        _pivot.localRotation = Quaternion.Euler(0f, 0f, nextAngle);

        if (_sr == null) return;

        float normalizedAngle = Mathf.DeltaAngle(0f, targetAngle);
        bool nextFlip = (normalizedAngle > 90f || normalizedAngle < -90f);

        if (nextFlip != IsFlipY)
        {
            IsFlipY = nextFlip;
            _sr.flipY = IsFlipY;

            Vector3 p = _pivot.localPosition;
            p.y = IsFlipY ? _pivotY_Left : _pivotY_Right;
            _pivot.localPosition = p;
        }
    }

    public void Freeze(float time)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(C_Freeze(time));
    }

    private IEnumerator C_Freeze(float time)
    {
        _isFrozen = true;

        if (_rb != null)
            _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(time);

        _isFrozen = false;
    }
}
