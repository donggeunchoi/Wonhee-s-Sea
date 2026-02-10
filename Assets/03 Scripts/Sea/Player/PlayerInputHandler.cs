using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerRoot _owner;

    [Header("Debug (PC 테스트용)")]
    [SerializeField] private bool _useKeyboardForDebug = true;

    [Header("공격 조이스틱")]
    [SerializeField] private FloatingJoystick _aimJoystick;
    
    private float _aimDeadZone = 0.15f;

    private Vector2 _joystickMove;
    private Vector2 _move;

    private Vector2 _lastNonZeroDir = Vector2.right;

    private bool _isAiming;
    private Vector2 _aimShootDir = Vector2.right;

    public bool IsAiming => _isAiming;

    public Vector2 Move => _move;

    public Vector2 FacingDir
    {
        get
        {
            if (_isAiming && _aimShootDir.sqrMagnitude > 0.0001f)
                return _aimShootDir.normalized;

            if (_lastNonZeroDir.sqrMagnitude > 0.0001f)
                return _lastNonZeroDir.normalized;

            return Vector2.right;
        }
    }

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        _joystickMove = Vector2.zero;
        _move = Vector2.zero;
        _lastNonZeroDir = Vector2.right;

        if (_aimJoystick != null)
        {
            _aimJoystick.OnReleased -= HandleAimReleased;
            _aimJoystick.OnReleased += HandleAimReleased;
        }
    }

    public void SetMove(Vector2 v)
    {
        if (_isAiming)
        {
            _joystickMove = Vector2.zero;
            return;
        }

        _joystickMove = v;
    }

    private void HandleAimReleased(Vector2 dir)
    {
        _owner?.Attack?.FireRanged(dir);
    }

    private void Update()
    {
        UpdateAimingState();

        Vector2 keyboard = Vector2.zero;
        if (_useKeyboardForDebug)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            keyboard = new Vector2(x, y);
            if (keyboard.sqrMagnitude > 1f) keyboard.Normalize();
        }

        if (_isAiming)
        {
            _move = Vector2.zero;
            return;
        }

        _move = (keyboard.sqrMagnitude > 0.0001f) ? keyboard : _joystickMove;

        if (_move.sqrMagnitude > 0.0001f)
            _lastNonZeroDir = _move.normalized;
    }

    private void UpdateAimingState()
    {
        if (_aimJoystick == null)
        {
            _isAiming = false;
            return;
        }

        Vector2 aimRaw = _aimJoystick.Direction;
        if (aimRaw.sqrMagnitude >= _aimDeadZone * _aimDeadZone)
        {
            _isAiming = true;

            _aimShootDir = (-aimRaw).normalized;

            _lastNonZeroDir = _aimShootDir;
        }
        else
        {
            _isAiming = false;
        }
    }
}
