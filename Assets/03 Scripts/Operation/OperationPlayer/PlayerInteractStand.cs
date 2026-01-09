using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractStand : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 1.5f;

    [SerializeField] private string testItemId;
    [SerializeField] private string testItemId1;
    [SerializeField] private int testAddCount = 5;
    
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI countText1;

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

        
        countText.text = inventory.CountOf(testItemId).ToString();
        countText1.text = inventory.CountOf(testItemId1).ToString();
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
            
            bool added = inventory.TryAdd(testItemId, testAddCount);
            Debug.Log($"text {testItemId} x {testAddCount} 지급시도 => {added}");
        }
        
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var hit = Physics2D.OverlapCircle(transform.position, range);
        if (hit == null) return;

        
        if (!hit.TryGetComponent<SashimiStand>(out var stand))
            return;
        
        stand.Interact(inventory);
    }
}
