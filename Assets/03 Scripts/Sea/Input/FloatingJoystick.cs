using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum JoystickMode { Floating, Fixed }

    [Header("Mode")]
    [SerializeField] private JoystickMode _mode = JoystickMode.Floating;

    [Header("UI")]
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;

    [Header("Settings")]
    [SerializeField] private float _radius = 80f;
    [SerializeField] private float _releaseDeadZone = 0.15f;

    private int? _pointerId;
    private Vector2 _lastDirection;
    private Vector3 _fixedBgWorldPos;
    private Vector3 _fixedHandleWorldPos;

    public Vector2 Direction { get; private set; }
    public bool HasDirection => Direction != Vector2.zero
        && Direction.sqrMagnitude >= _releaseDeadZone * _releaseDeadZone;

    public event Action<Vector2> OnReleased;
    public event Action OnCanceled;

    private void Awake()
    {
        if (_background == null || _handle == null)
        {
            enabled = false;
            return;
        }

        _fixedBgWorldPos = _background.position;
        _fixedHandleWorldPos = _handle.position;
        ResetInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActiveAndEnabled || _pointerId.HasValue) return;
        _pointerId = eventData.pointerId;

        _background.gameObject.SetActive(true);
        _handle.gameObject.SetActive(true);

        if (_mode == JoystickMode.Floating)
        {
            _background.position = eventData.position;
            _handle.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isActiveAndEnabled || eventData.pointerId != _pointerId || _radius <= 0f)
            return;

        Vector2 backgroundPos = _background.position;
        Vector2 offset = eventData.position - backgroundPos;
        Vector2 clamped = Vector2.ClampMagnitude(offset, _radius);

        _handle.position = backgroundPos + clamped;
        Direction = clamped / _radius;

        if (HasDirection)
            _lastDirection = Direction.normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != _pointerId) return;

        Vector2 releasedDirection = _lastDirection;
        ResetInput();
        OnReleased?.Invoke(releasedDirection);
    }

    public void CancelInput()
    {
        if (!_pointerId.HasValue) return;

        ResetInput();
        OnCanceled?.Invoke();
    }

    private void OnDisable()
    {
        CancelInput();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            CancelInput();
    }

    private void ResetInput()
    {
        _pointerId = null;
        Direction = Vector2.zero;
        _lastDirection = Vector2.zero;

        if (_mode == JoystickMode.Floating)
        {
            _background.gameObject.SetActive(false);
            _handle.gameObject.SetActive(false);
        }
        else
        {
            _background.gameObject.SetActive(true);
            _handle.gameObject.SetActive(true);
            _background.position = _fixedBgWorldPos;
            _handle.position = _fixedHandleWorldPos;
        }
    }
}
