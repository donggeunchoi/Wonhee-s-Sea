using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;

    [SerializeField] private string fishItemId;
    [SerializeField] private string sashimiItemId;
    [SerializeField] private string rearRiceItemId;
    [SerializeField] private string evenRiceItemId;
    [SerializeField] private string sushiItemId;
    [SerializeField] private int testAddCount;
    
    [SerializeField] private TextMeshProUGUI fishCount;
    [SerializeField] private TextMeshProUGUI sashimiCount;
    [SerializeField] private TextMeshProUGUI rearRiceCount;
    [SerializeField] private TextMeshProUGUI evenRiceCount;
    [SerializeField] private TextMeshProUGUI sushiCount;
    

    void Start()
    {
        if (inventory != null)
        {
            inventory.OnChange += Refresh;

            Refresh();
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChange -= Refresh;
    }

    private void Refresh()
    {
        if (inventory == null)
            return;

        
        fishCount.text = inventory.CountOf(fishItemId).ToString();
        sashimiCount.text = inventory.CountOf(sashimiItemId).ToString();
        rearRiceCount.text = inventory.CountOf(rearRiceItemId).ToString();
        evenRiceCount.text = inventory.CountOf(evenRiceItemId).ToString();
        sushiCount.text = inventory.CountOf(evenRiceItemId).ToString();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (inventory == null)
            {
                Debug.LogWarning("인벤토리 연결 안됨");
                return;
            }
            
            bool fishadded = inventory.TryAdd(fishItemId, testAddCount);
            bool rearRiceadded = inventory.TryAdd(rearRiceItemId, testAddCount);
            Debug.Log($"text {fishItemId} x {testAddCount} 지급시도 => {fishadded}");
            Debug.Log($"text {rearRiceItemId} x {testAddCount} 지급시도 => {rearRiceadded}");
        }
        
        var hit = Physics2D.OverlapCircle(transform.position, range);
        if (hit == null) return;

        
        if (!hit.TryGetComponent<RecipeStand>(out var stand))
            return;
        
        stand.Interact(inventory);
    }
}
