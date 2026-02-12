using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Aim Settings")]
    [SerializeField] private float _releaseDeadZone = 0.15f;

    private int _pointerId = -1;

    private Vector2 _input;
    public Vector2 Direction => _input;

    private Vector2 _lastNonZeroDir = Vector2.right;
    public event Action<Vector2> OnReleased;

    private Vector3 _fixedBgWorldPos;
    private Vector3 _fixedHandleWorldPos;

    private void Awake()
    {
        if (_background == null || _handle == null)
        {
            enabled = false;
            return;
        }

        _fixedBgWorldPos = _background.position;
        _fixedHandleWorldPos = _handle.position;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pointerId != -1) return;
        _pointerId = eventData.pointerId;

        _background.gameObject.SetActive(true);
        _handle.gameObject.SetActive(true);

        if (_mode == JoystickMode.Floating)
        {
            _background.position = eventData.position;
            _handle.position = eventData.position;
        }
        else
        {
            _background.position = _fixedBgWorldPos;
            _handle.position = _fixedHandleWorldPos;
        }

        _input = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != _pointerId) return;
        if (_radius <= 0f) return;

        Vector2 bgPos = _background.position;
        Vector2 dragPos = eventData.position;

        Vector2 dir = dragPos - bgPos;
        Vector2 clamped = Vector2.ClampMagnitude(dir, _radius);

        _handle.position = bgPos + clamped;

        _input = clamped / _radius;
        if (float.IsNaN(_input.x) || float.IsNaN(_input.y))
            _input = Vector2.zero;

        if (_input.sqrMagnitude >= _releaseDeadZone * _releaseDeadZone)
            _lastNonZeroDir = _input.normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != _pointerId) return;

        _pointerId = -1;

        _input = Vector2.zero;

        OnReleased?.Invoke(_lastNonZeroDir);

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
