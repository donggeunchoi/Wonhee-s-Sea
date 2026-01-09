using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : Singleton<JoystickManager>
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private FloatingJoystick _joystick;

    private void Update()
    {
        if (_player == null || _joystick == null) return;
        
        _player.SetMoveInput(_joystick.Direction);
       
    }
}
