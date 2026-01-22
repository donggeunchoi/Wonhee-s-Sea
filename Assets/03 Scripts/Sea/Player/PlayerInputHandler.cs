using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerRoot _owner;

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
        _move = Vector2.zero;
        _lastNonZeroDir = Vector2.right;
    }

    public void SetMove(Vector2 v)
    {
        _move = v;

        if (_move.sqrMagnitude > 0.0001f)
            _lastNonZeroDir = _move.normalized;
    }
}
