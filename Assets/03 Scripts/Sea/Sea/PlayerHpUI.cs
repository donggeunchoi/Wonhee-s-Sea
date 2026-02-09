using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    [Header("UI")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private CanvasGroup _flashGroup;
    [SerializeField] private TextMeshProUGUI _text;

    private Coroutine _flashCo;
    private int _prevHp = -1;
    private float _flashOnSeconds = 0.12f;
    private float _flashFadeSeconds = 0.20f;

    private void Awake()
    {
        if (_stats == null) _stats = FindAnyObjectByType<PlayerStats>();

        if (_flashGroup != null) _flashGroup.alpha = 0f;
        if (_fillImage != null) _fillImage.fillAmount = 1f;
    }

    private void Start()
    {
        if (_flashGroup != null)
            _flashGroup.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (_stats != null)
            _stats.OnHpChanged += HandleHpChanged;

        if (_stats != null)
        {
            _prevHp = _stats.CurrentHp;
            HandleHpChanged(_stats.CurrentHp, _stats.MaxHp);
        }
    }

    private void OnDisable()
    {
        if (_stats != null)
            _stats.OnHpChanged -= HandleHpChanged;

        if (_flashCo != null) StopCoroutine(_flashCo);
        _flashCo = null;

        if (_flashGroup != null) _flashGroup.alpha = 0f;
    }

    private void HandleHpChanged(int current, int max)
    {
        float t = (max <= 0) ? 0f : (float)current / max;
        if (_fillImage != null) _fillImage.fillAmount = Mathf.Clamp01(t);

        if (_text != null) _text.text = $"{current}/{max}";

        if (_prevHp >= 0 && current < _prevHp)
            FlashOnce();

        _prevHp = current;
    }

    private void FlashOnce()
    {
        if (_flashGroup == null) return;

        if (_flashCo != null) StopCoroutine(_flashCo);
        _flashCo = StartCoroutine(C_FlashOnce());
    }

    private IEnumerator C_FlashOnce()
    {
        _flashGroup.alpha = 1f;
        yield return new WaitForSeconds(_flashOnSeconds);

        float t = 0f;
        float start = 1f;

        while (t < _flashFadeSeconds)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, 0f, t / _flashFadeSeconds);
            _flashGroup.alpha = a;
            yield return null;
        }

        _flashGroup.alpha = 0f;
        _flashCo = null;
    }
}
