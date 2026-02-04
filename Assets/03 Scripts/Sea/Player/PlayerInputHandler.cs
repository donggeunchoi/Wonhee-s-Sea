using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerRoot _owner;

    [Header("Debug (PC 테스트용)")]
    [SerializeField] private bool _useKeyboardForDebug = true;

    private Vector2 _joystickMove;
    private Vector2 _move;
    private Vector2 _lastNonZeroDir = Vector2.right;

    public Vector2 Move => _move;

    public Vector2 FacingDir
    {
        get
        {
            if (_lastNonZeroDir.sqrMagnitude > 0.0001f) return _lastNonZeroDir.normalized;
            return Vector2.right;
        }
    }

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _joystickMove = Vector2.zero;
        _move = Vector2.zero;
        _lastNonZeroDir = Vector2.right;
    }

    public void SetMove(Vector2 v)
    {
        _joystickMove = v;
    }

    private void Update()
    {
        Vector2 keyboard = Vector2.zero;
        if (_useKeyboardForDebug)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            keyboard = new Vector2(x, y);
            if (keyboard.sqrMagnitude > 1f) keyboard.Normalize();
        }

        _move = (keyboard.sqrMagnitude > 0.0001f) ? keyboard : _joystickMove;

        if (_move.sqrMagnitude > 0.0001f)
            _lastNonZeroDir = _move.normalized;
    }
}
