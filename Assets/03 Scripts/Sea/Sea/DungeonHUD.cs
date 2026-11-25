using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonHUD : MonoBehaviour
{
    [SerializeField] private RectTransform _safeArea;

    [Header("Oxygen")]
    [SerializeField] private PlayerOxygen _oxygen;
    [SerializeField] private TMP_Text _oxygenText;
    [SerializeField] private RectTransform _oxygenFill;

    [Header("Health")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private CanvasGroup _damageFlash;

    [Header("Circular Vitals")]
    [SerializeField] private Image _vitalsHealthRing;
    [SerializeField] private RectTransform _vitalsOxygenWater;
    [SerializeField] private TMP_Text _vitalsOxygenText;


    [Header("Depth and Catch")]
    [SerializeField] private DepthCalculator _depth;
    [SerializeField] private TMP_Text _depthText;
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private TMP_Text _catchText;

    private Rect _lastSafeArea;
    private Vector2Int _lastScreen;
    private float _previousHp;
    private Coroutine _flash;

    private void OnEnable()
    {
        _previousHp = _stats.CurrentHp;
        _stats.OnHpChanged += HandleHpChanged;
        _damageFlash.alpha = 0f;
        _damageFlash.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _stats.OnHpChanged -= HandleHpChanged;
        if (_flash != null)
            StopCoroutine(_flash);

        _flash = null;
        _damageFlash.alpha = 0f;
    }

    private void LateUpdate()
    {
        UpdateSafeArea();

        float oxygen = _oxygen.CurrentOxygen;
        float maxOxygen = _oxygen.MaxOxygen;
        float ratio = maxOxygen > 0f ? Mathf.Clamp01(oxygen / maxOxygen) : 0f;
        _oxygenText.SetText("{0:0}/{1:0}", Mathf.CeilToInt(oxygen), maxOxygen);
        _oxygenText.color = ratio <= 0.2f
            ? new Color32(255, 140, 125, 255)
            : new Color32(255, 239, 203, 255);
        _oxygenFill.anchorMax = new Vector2(ratio, 1f);

        float hp = _stats.CurrentHp;
        float maxHp = _stats.MaxHp;
        _healthText.SetText("{0:0}/{1:0}", Mathf.CeilToInt(hp), maxHp);

        if (_vitalsHealthRing != null)
        {
            float healthRatio = maxHp > 0f ? Mathf.Clamp01(hp / maxHp) : 0f;
            _vitalsHealthRing.fillAmount = healthRatio;
        }

        if (_vitalsOxygenWater != null && _vitalsOxygenWater.parent is RectTransform tank)
        {
            float halfTankHeight = tank.rect.height * 0.5f;
            float waveTopMargin = _vitalsOxygenWater.rect.height * 0.06f;
            Vector2 waterPosition = _vitalsOxygenWater.anchoredPosition;
            waterPosition.y = Mathf.Lerp(-halfTankHeight, halfTankHeight + waveTopMargin, ratio);
            _vitalsOxygenWater.anchoredPosition = waterPosition;
        }

        if (_vitalsOxygenText != null)
            _vitalsOxygenText.SetText("{0:0}/{1:0}", Mathf.CeilToInt(oxygen), maxOxygen);

        _depthText.SetText("{0:0} m", Mathf.FloorToInt(_depth.CurrentDepth));
        _catchText.SetText("{0:0.0}/{1:0.0} kg", _inventory.CurrentWeight, _inventory.MaxWeight);
    }

    private void UpdateSafeArea()
    {
        Rect safe = Screen.safeArea;
        var screen = new Vector2Int(Screen.width, Screen.height);
        if (screen.x <= 0 || screen.y <= 0 || (safe == _lastSafeArea && screen == _lastScreen))
            return;

        _lastSafeArea = safe;
        _lastScreen = screen;
        _safeArea.anchorMin = new Vector2(safe.xMin / screen.x, safe.yMin / screen.y);
        _safeArea.anchorMax = new Vector2(safe.xMax / screen.x, safe.yMax / screen.y);
        _safeArea.offsetMin = _safeArea.offsetMax = Vector2.zero;
    }

    private void HandleHpChanged(float current, float max)
    {
        if (current < _previousHp)
        {
            if (_flash != null)
                StopCoroutine(_flash);

            _flash = StartCoroutine(FlashDamage());
        }

        _previousHp = current;
    }

    private IEnumerator FlashDamage()
    {
        _damageFlash.alpha = 1f;
        yield return new WaitForSecondsRealtime(0.12f);

        const float fadeDuration = 0.20f;
        for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.unscaledDeltaTime)
        {
            _damageFlash.alpha = 1f - elapsed / fadeDuration;
            yield return null;
        }

        _damageFlash.alpha = 0f;
        _flash = null;
    }
}
