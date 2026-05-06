using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSensor : MonoBehaviour
{
    private FishRoot _owner;
    private Transform _targetTr;

    public Transform TargetTr => _targetTr;

    public event Action<Transform> OnTargetEnter;
    public event Action OnTargetExit;

    public void Init(FishRoot owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_owner == null) return;
        if (!other.CompareTag("Player")) return;

        _targetTr = other.transform;
        OnTargetEnter?.Invoke(_targetTr);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_owner == null) return;
        if (!other.CompareTag("Player")) return;
        if (_targetTr == null) return;
        if (other.transform != _targetTr) return;

        _targetTr = null;
        OnTargetExit?.Invoke();
    }
}
