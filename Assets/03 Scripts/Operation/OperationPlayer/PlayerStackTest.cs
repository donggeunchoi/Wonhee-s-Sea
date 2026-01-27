using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStackTest : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [Header("테스트 아이템")]
    [SerializeField] private string itemId1;
    [SerializeField] private string itemId2;

    [SerializeField] private int addCount = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (inventory == null)
            {
                Debug.LogWarning("Inventory 연결 안됨");
                return;
            }

            inventory.TryAdd(itemId1, addCount);
            inventory.TryAdd(itemId2, addCount);
        }
    }
}
