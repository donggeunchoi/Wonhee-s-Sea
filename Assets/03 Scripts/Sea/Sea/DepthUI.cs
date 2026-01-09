using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DepthUI : MonoBehaviour
{
    [SerializeField] private DepthCalculator _depthCalculator;
    [SerializeField] private TMP_Text _depthText;

    private void Update()
    {
        UpdateDepthText();
    }

    private void UpdateDepthText()
    {
        _depthText.text = $"{Mathf.FloorToInt(_depthCalculator.CurrentDepth)} m";
    }
}
