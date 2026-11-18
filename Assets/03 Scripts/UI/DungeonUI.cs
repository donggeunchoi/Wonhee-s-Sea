using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    [Header("Depth")]
    [SerializeField] private DepthCalculator depthCalculator;
    [SerializeField] private TMP_Text depthText;
    
    [Header("Weight")]
    [SerializeField] private PlayerInventory player_Inventory;
    [SerializeField] private TextMeshProUGUI weight_text;
    [SerializeField] private TextMeshProUGUI maxweight_text;
    
    [Header("Oxygen")]
    [SerializeField] private PlayerOxygen player_Oxygen;
    [SerializeField] private Image Oxygen_fill;
    [SerializeField] private GameObject Oxygen_warnObj;
    [SerializeField] private TextMeshProUGUI Oxygen_text;

    private bool _wasBelowThreshold;
    private Coroutine _warnCo;
    private float _warnThreshold = 10f;
    private float _warnShowSeconds = 0.8f;
    
    
    private void Awake()
    {
        //무게
        if (player_Inventory == null)
            player_Inventory = FindAnyObjectByType<PlayerInventory>();
        
        //산소
        if (player_Oxygen == null) player_Oxygen = FindAnyObjectByType<PlayerOxygen>();
        if (Oxygen_warnObj != null) Oxygen_warnObj.SetActive(false);
    }

    private void OnEnable()
    {
        //무게
        if (player_Inventory != null)
            player_Inventory.OnWeightChanged += HandleWeightChanged;

        if (player_Inventory != null)
            HandleWeightChanged(player_Inventory.CurrentWeight, player_Inventory.MaxWeight);
        
        //산소
        if (player_Oxygen != null)
            player_Oxygen.OnOxygenChanged += HandlePlayerOxygenChanged;

        if (player_Oxygen != null)
            HandlePlayerOxygenChanged(player_Oxygen.CurrentOxygen, player_Oxygen.MaxOxygen);
    }

    private void OnDisable()
    {
        //무게
        if (player_Inventory != null)
            player_Inventory.OnWeightChanged -= HandleWeightChanged;
        
        
        //산소
        if (player_Oxygen != null)
            player_Oxygen.OnOxygenChanged -= HandlePlayerOxygenChanged;

        if (_warnCo != null) StopCoroutine(_warnCo);
        _warnCo = null;

        if (Oxygen_warnObj != null) Oxygen_warnObj.SetActive(false);
    }
    
    private void Update()
    {
        UpdateDepthText();
    }

    //수심
    private void UpdateDepthText()
    {
        depthText.text = $"{Mathf.FloorToInt(depthCalculator.CurrentDepth)} M";
    }
    
    //무게
    private void HandleWeightChanged(float current, float max)
    {
        float t = (max <= 0) ? 0f : current / max;

        if (weight_text != null)
            weight_text.text = $"{current:F1} kg";
        
        if (maxweight_text != null)
            maxweight_text.text = $"/{max:F1} kg";
    }
    
    //산소
    private void HandlePlayerOxygenChanged(float current, float max)
    {
        float t = (max <= 0f) ? 0f : current / max;
        if (Oxygen_fill != null) Oxygen_fill.fillAmount = Mathf.Clamp01(t);

        if (Oxygen_text != null) Oxygen_text.text = $"{Mathf.CeilToInt(current)}";

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
        if (Oxygen_warnObj == null) return;

        if (_warnCo != null) StopCoroutine(_warnCo);
        _warnCo = StartCoroutine(C_ShowWarn());
    }

    private IEnumerator C_ShowWarn()
    {
        Oxygen_warnObj.SetActive(true);
        yield return new WaitForSeconds(_warnShowSeconds);
        Oxygen_warnObj.SetActive(false);
        _warnCo = null;
    }
}
