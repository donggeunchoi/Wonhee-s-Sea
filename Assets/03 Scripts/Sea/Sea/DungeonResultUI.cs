using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private TMP_Text _totalCountText;
    [SerializeField] private TMP_Text _totalWeightText;

    private void Awake()
    {
        _panel.SetActive(false);
    }

    public void Open()
    {
        _panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        _panel.SetActive(false);
    }

    private void Refresh()
    {
        int totalCount = _inventory.FishList.Count;
        float totalWeight = _inventory.CurrentWeight;

        _totalCountText.text = $"{totalCount}";
        _totalWeightText.text = $"{totalWeight:0.0} / {_inventory.MaxWeight:0.0}";
    }

    public void OnClickReturnTown()
    {
        SceneManager.LoadScene("TownScene");
    }
}
