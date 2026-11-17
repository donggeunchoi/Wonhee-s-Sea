using TMPro;
using UnityEngine;

public class DungeonResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _totalCountText;
    [SerializeField] private TMP_Text _totalWeightText;

    private bool _shown;

    private void Awake()
    {
        if (!_shown)
            _panel.SetActive(false);
    }

    public void Show(int fishCount, float weight, float maxWeight)
    {
        _shown = true;
        _totalCountText.text = $"{fishCount}";
        _totalWeightText.text = $"{weight:0.0} / {maxWeight:0.0}";
        _panel.SetActive(true);
    }
}
