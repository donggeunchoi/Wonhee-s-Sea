using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private bool _useKeyboardForDebug = true;
    [SerializeField] private FloatingJoystick _moveJoystick;

    public Vector2 Move { get; private set; }

    private void Update()
    {
        Move = ReadMoveInput();
    }

    public Vector2 ReadMoveInput()
    {
        if (!isActiveAndEnabled) return Vector2.zero;

        Vector2 keyboard = Vector2.zero;

        if (_useKeyboardForDebug)
        {
            keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            keyboard = Vector2.ClampMagnitude(keyboard, 1f);
        }

        return keyboard.sqrMagnitude > 0.0001f
            ? keyboard
            : (_moveJoystick != null ? _moveJoystick.Direction : Vector2.zero);
    }

    private void OnDisable()
    {
        Move = Vector2.zero;
        if (_moveJoystick != null)
            _moveJoystick.CancelInput();
    }
}
