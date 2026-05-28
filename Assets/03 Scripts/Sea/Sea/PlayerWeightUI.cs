using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWeightUI : MonoBehaviour
{
    /*[SerializeField] private PlayerInventory _inventory;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        if (_inventory == null)
            _inventory = FindAnyObjectByType<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (_inventory != null)
            _inventory.OnWeightChanged += HandleWeightChanged;

        if (_inventory != null)
            HandleWeightChanged(_inventory.CurrentWeight, _inventory.MaxWeight);
    }

    private void OnDisable()
    {
        if (_inventory != null)
            _inventory.OnWeightChanged -= HandleWeightChanged;
    }

    private void HandleWeightChanged(float current, float max)
    {
        float t = (max <= 0) ? 0f : current / max;

        if (_text != null)
            _text.text = $"{current:F1} / {max:F1}";
    }*/
}

