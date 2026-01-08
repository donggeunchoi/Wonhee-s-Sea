using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemStack
{
    public string itemId;
    public int count;
    public bool IsEmpty => string.IsNullOrEmpty(itemId) || count <= 0;

    public void Clear()
    {
        itemId = null;
        count = 0;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("아이템 데이터 베이스")]
    [SerializeField] 
    private ItemDatabaseSO dataBase;
    
    [Header("시작시 가지게 될 슬롯 개수")]
    [SerializeField] 
    private int startSlot = 5;

    public event Action OnChange;

    public List<ItemStack> Slots { get; private set; } = new List<ItemStack>();

    private void Awake()
    {
        Slots = new List<ItemStack>(startSlot);
        for (int i = 0; i < startSlot; i++)
        {
            Slots.Add(new ItemStack());
            OnChange?.Invoke();
        }
    }

    public void ExpandSlots(int add)
    {
        for (int i = 0; i < add; i++)
        {
            Slots.Add(new ItemStack());
            OnChange?.Invoke();
        }
    }

    public int CountOf(string itemId)
    {
        int sum = 0;
        for (int i = 0; i < Slots.Count; i++)
            if (Slots[i].itemId == itemId) sum += Slots[i].count;
        return sum;
    }
    
    public bool TryRemove(string itemId, int amount)
    {
        if (amount <= 0) return true;
        if (CountOf(itemId) < amount) return false;

        int left = amount;
        for (int i = 0; i < Slots.Count && left > 0; i++)
        {
            var s = Slots[i];
            if (s.itemId != itemId) continue;

            int take = Mathf.Min(s.count, left);
            s.count -= take;
            left -= take;

            if (s.count <= 0) s.Clear();
            Slots[i] = s;
        }

        OnChange?.Invoke();
        return true;
    }
    
    public bool TryAdd(string itemId, int amount)
    {
        if (amount <= 0) return true;
        if (dataBase == null) { Debug.LogError("Inventory: db 미할당"); return false; }

        bool stackable = dataBase.IsStackable(itemId);
        int maxStack = dataBase.GetMaxStack(itemId);

        int left = amount;

        // 1) 기존 스택에 합치기
        if (stackable)
        {
            for (int i = 0; i < Slots.Count && left > 0; i++)
            {
                var s = Slots[i];
                if (s.IsEmpty || s.itemId != itemId) continue;

                int canPut = maxStack - s.count;
                if (canPut <= 0) continue;

                int put = Mathf.Min(canPut, left);
                s.count += put;
                left -= put;
                Slots[i] = s;
            }
        }

        // 2) 빈 슬롯에 넣기
        for (int i = 0; i < Slots.Count && left > 0; i++)
        {
            var s = Slots[i];
            if (!s.IsEmpty) continue;

            s.itemId = itemId;
            int put = stackable ? Mathf.Min(maxStack, left) : 1;
            s.count = put;
            left -= put;
            Slots[i] = s;

            if (!stackable && left > 0) // 비스택이면 슬롯 더 필요
                continue;
        }

        if (left > 0) return false; // 공간 부족
        OnChange?.Invoke();
        return true;
    }
}
