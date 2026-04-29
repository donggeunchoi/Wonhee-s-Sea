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
            if (inventory == null)
            {
                Debug.LogWarning("Inventory 연결 안됨");
                return;
            }

            int before1 = itemId1 % 10000;
            int before2 = itemId2 % 10000;

            string after1 = string.Format("{0:00}", itemId1);
            string after2 = string.Format("{0:00}", itemId2);

            inventory.TryAdd(after1, addCount);
            inventory.TryAdd(after2, addCount);
        }
    }
}
