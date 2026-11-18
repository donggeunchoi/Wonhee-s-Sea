using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : MonoBehaviour
{
    [SerializeField] private PlayerRoot _player;
    [SerializeField] private FloatingJoystick _joystick;

    private void Update()
    {
        if (_player == null || _joystick == null) return;

        _player.Input?.SetMove(_joystick.Direction);
    }
}
