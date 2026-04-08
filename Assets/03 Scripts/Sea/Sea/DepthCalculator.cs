using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCalculator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _surfaceY = 0f;

    public float CurrentDepth { get; private set; }

    private void Update()
    {
        CalculateDepth();
    }

    private void CalculateDepth()
    {
        if (_player == null) return;

        float depth = _surfaceY - _player.position.y;
        CurrentDepth = Mathf.Max(0f, depth);
    }

    public float GetDepthByWorldY(float worldY)
    {
        float depth = _surfaceY - worldY;
        return Mathf.Max(0f, depth);
    }
}
