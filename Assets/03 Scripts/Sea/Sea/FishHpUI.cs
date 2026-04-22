using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishHpUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _visibleTime = 2f;

    private FishStats _stats;
    private Coroutine _hideRoutine;

    public void Init(FishStats stats)
    {
        if (_stats != null)
            _stats.OnDamaged -= OnDamaged;

        _stats = stats;

        if (_stats == null)
        {
            enabled = false;
            return;
        }

        _stats.OnDamaged += OnDamaged;

        if (_fillImage != null)
            _fillImage.fillAmount = 1f;

        gameObject.SetActive(false);
    }

    private void OnDamaged(float current, float max)
    {
        if (_fillImage == null) return;
        if (max <= 0f) return;

        float ratio = Mathf.Clamp01(current / max);
        _fillImage.fillAmount = ratio;

        gameObject.SetActive(true);

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(C_HideAfterDelay());
    }

    private IEnumerator C_HideAfterDelay()
    {
        yield return new WaitForSeconds(_visibleTime);

        gameObject.SetActive(false);
        _hideRoutine = null;
    }

    private void OnDestroy()
    {
        if (_stats != null)
            _stats.OnDamaged -= OnDamaged;
    }
}