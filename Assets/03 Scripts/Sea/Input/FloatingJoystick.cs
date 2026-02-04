using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;

    [Header("Settings")]
    [SerializeField] private float _radius = 80f;

    private int _pointerId = -1;

    private Vector2 _input;
    public Vector2 Direction => _input;


    private void Awake()
    {
        if (_background == null || _handle == null)
        {
            enabled = false;
            return;
        }

        _background.gameObject.SetActive(false);
        _handle.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pointerId != -1) return;
        _pointerId = eventData.pointerId;

        _background.gameObject.SetActive(true);
        _handle.gameObject.SetActive(true);

        _background.position = eventData.position;
        _handle.position = eventData.position;

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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != _pointerId) return;
        _pointerId = -1;

        _background.gameObject.SetActive(false);
        _handle.gameObject.SetActive(false);
        _input = Vector2.zero;
    }
}

