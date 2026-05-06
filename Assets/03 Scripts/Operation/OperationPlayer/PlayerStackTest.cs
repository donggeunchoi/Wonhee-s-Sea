using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStackTest : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemRuntimeDatabase itemRuntimeDatabase;

    [Header("테스트 아이템")]
    [SerializeField] private int itemId1;
    [SerializeField] private int itemId2;
    [SerializeField] private int addCount = 3;

    private bool isInitialized = false;

    private void Start()
    {
        if (itemRuntimeDatabase == null)
            itemRuntimeDatabase = FindObjectOfType<ItemRuntimeDatabase>();

        if (itemRuntimeDatabase == null)
        {
            Debug.LogError("ItemRuntimeDatabase를 찾지 못함");
            return;
        }

        List<ItemDataTable> items = JsonManager.Ins.GetJson<ItemDataTable>();
        if (items == null || items.Count == 0)
        {
            Debug.LogError("ItemDataTable 데이터가 비어있음");
            return;
        }

        itemRuntimeDatabase.SetItems(items);
        isInitialized = true;

        Debug.Log("PlayerStackTest: ItemRuntimeDatabase 초기화 완료");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[PlayerStackTest] T 입력 감지");

            if (inventory == null)
            {
                Debug.LogWarning("Inventory 연결 안됨");
                return;
            }

            string itemIdString1 = itemId1.ToString();
            string itemIdString2 = itemId2.ToString();

            bool result1 = inventory.TryAdd(itemIdString1, addCount);
            bool result2 = inventory.TryAdd(itemIdString2, addCount);

            Debug.Log($"[PlayerStackTest] Add 결과 / itemId1={itemIdString1}, count={addCount}, result={result1}, 현재개수={inventory.CountOf(itemIdString1)}");
            Debug.Log($"[PlayerStackTest] Add 결과 / itemId2={itemIdString2}, count={addCount}, result={result2}, 현재개수={inventory.CountOf(itemIdString2)}");
        }
    }
}
