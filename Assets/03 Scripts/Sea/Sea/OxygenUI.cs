using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OxygenUI : MonoBehaviour
{
    [SerializeField] private Oxygen _oxygen;

    [Header("UI")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private GameObject _warnObj;
    [SerializeField] private TextMeshProUGUI _text;

    private bool _wasBelowThreshold;
    private Coroutine _warnCo;
    private float _warnThreshold = 10f;
    private float _warnShowSeconds = 0.8f;

    private void Awake()
    {
        if (_oxygen == null) _oxygen = FindAnyObjectByType<Oxygen>();
        if (_warnObj != null) _warnObj.SetActive(false);
    }

    private void OnEnable()
    {
        if (_oxygen != null)
            _oxygen.OnOxygenChanged += HandleOxygenChanged;

        if (_oxygen != null)
            HandleOxygenChanged(_oxygen.CurrentOxygen, _oxygen.MaxOxygen);
    }

    private void OnDisable()
    {
        if (_oxygen != null)
            _oxygen.OnOxygenChanged -= HandleOxygenChanged;

        if (_warnCo != null) StopCoroutine(_warnCo);
        _warnCo = null;

        if (_warnObj != null) _warnObj.SetActive(false);
    }

    private void HandleOxygenChanged(float current, float max)
    {
        float t = (max <= 0f) ? 0f : current / max;
        if (_fillImage != null) _fillImage.fillAmount = Mathf.Clamp01(t);

        if (_text != null) _text.text = $"{Mathf.CeilToInt(current)}";

        bool isBelow = (current > 0f && current <= _warnThreshold);

        if (isBelow && !_wasBelowThreshold)
            ShowWarningOnce();

        if (!isBelow)
            _wasBelowThreshold = false;
        else
            _wasBelowThreshold = true;
    }

    private void ShowWarningOnce()
    {
        if (_warnObj == null) return;

        if (_warnCo != null) StopCoroutine(_warnCo);
        _warnCo = StartCoroutine(C_ShowWarn());
    }

    private IEnumerator C_ShowWarn()
    {
        _warnObj.SetActive(true);
        yield return new WaitForSeconds(_warnShowSeconds);
        _warnObj.SetActive(false);
        _warnCo = null;
    }
}
