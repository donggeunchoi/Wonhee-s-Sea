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
        _stats = stats;
        _stats.OnDamaged += OnDamaged;

        gameObject.SetActive(false);
    }

    private void OnDamaged(int current, int max)
    {
        float ratio = (float)current / max;
        _fillImage.fillAmount = ratio;

        gameObject.SetActive(true);

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_visibleTime);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_stats != null)
            _stats.OnDamaged -= OnDamaged;
    }
}