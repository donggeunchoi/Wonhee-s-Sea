using System;
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
    [Header("아이템 프리팹 조회용")]
    [SerializeField] private ItemAssetManager itemAssetManager;

    [Header("시작시 가지게 될 슬롯 개수(=수용량)")]
    [SerializeField] private int startSlot = 10;

    [Header("머리 위 기준점")]
    [SerializeField] private Transform stackRoot;

    [Header("스택 간격(프리팹 1개당 올라갈 높이)")]
    [SerializeField] private float offsetY = 0.2f;

    [Header("머리위에 표시할지 여부")]
    [SerializeField] private bool useHeadStackVisual = true;

    [Header("기본 최대 스택 수")]
    [SerializeField] private int defaultMaxStack = 999;

    public event Action OnChange;

    public List<ItemStack> Slots { get; private set; } = new List<ItemStack>();

    [Serializable]
    private class VisualEntry
    {
        public string itemId;
        public GameObject obj;
    }

    [Header("디버그: 머리 위에 쌓인 리스트(런타임 확인용)")]
    [SerializeField] private List<VisualEntry> visualStack = new List<VisualEntry>();

    public int Capacity => Slots.Count;
    public int VisualCount => visualStack.Count;

    private void Awake()
    {
        Slots = new List<ItemStack>(startSlot);

        for (int i = 0; i < startSlot; i++)
            Slots.Add(new ItemStack());

        SyncVisualFromSlots();
        OnChange?.Invoke();
    }

    public void ExpandSlots(int add)
    {
        if (add <= 0)
            return;

        for (int i = 0; i < add; i++)
            Slots.Add(new ItemStack());

        OnChange?.Invoke();
    }

    public int CountOf(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return 0;

        int sum = 0;

        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].itemId == itemId)
                sum += Slots[i].count;
        }

        return sum;
    }

    public bool Has(string itemId, int amount = 1)
    {
        return CountOf(itemId) >= amount;
    }

    public bool TryAdd(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId))
            return false;

        if (amount <= 0)
            return true;

        int before = CountOf(itemId);
        int left = amount;

        // 같은 itemId 슬롯에 먼저 채우기
        for (int i = 0; i < Slots.Count && left > 0; i++)
        {
            var s = Slots[i];

            if (s.IsEmpty || s.itemId != itemId)
                continue;

            int canPut = defaultMaxStack - s.count;
            if (canPut <= 0)
                continue;

            int put = Mathf.Min(canPut, left);
            s.count += put;
            left -= put;
            Slots[i] = s;
        }

        // 빈 슬롯에 새로 넣기
        for (int i = 0; i < Slots.Count && left > 0; i++)
        {
            var s = Slots[i];

            if (!s.IsEmpty)
                continue;

            s.itemId = itemId;

            int put = Mathf.Min(defaultMaxStack, left);
            s.count = put;
            left -= put;

            Slots[i] = s;
        }

        if (left > 0)
            return false;

        int after = CountOf(itemId);
        int added = after - before;

        if (useHeadStackVisual && added > 0)
            VisualAdd(itemId, added);

        OnChange?.Invoke();
        return true;
    }

    public bool TryRemove(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId))
            return false;

        if (amount <= 0)
            return true;

        int before = CountOf(itemId);
        if (before < amount)
            return false;

        int left = amount;

        for (int i = 0; i < Slots.Count && left > 0; i++)
        {
            var s = Slots[i];

            if (s.itemId != itemId)
                continue;

            int take = Mathf.Min(s.count, left);
            s.count -= take;
            left -= take;

            if (s.count <= 0)
                s.Clear();

            Slots[i] = s;
        }

        int after = CountOf(itemId);
        int removed = before - after;

        if (useHeadStackVisual && removed > 0)
            VisualRemoveById(itemId, removed);

        OnChange?.Invoke();
        return true;
    }

    private void VisualAdd(string itemId, int amount)
    {
        if (!useHeadStackVisual)
            return;

        if (stackRoot == null)
        {
            Debug.LogWarning("[Inventory] stackRoot 미할당", this);
            return;
        }

        if (itemAssetManager == null)
        {
            Debug.LogWarning("[Inventory] itemAssetManager 미할당", this);
            return;
        }

        GameObject prefab = itemAssetManager.GetPrefab(itemId);
        if (prefab == null)
        {
            Debug.LogWarning($"[Inventory] prefab 없음. itemId={itemId}", this);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab, stackRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            visualStack.Add(new VisualEntry
            {
                itemId = itemId,
                obj = obj
            });
        }

        RepositionVisual();
    }

    private int VisualRemoveById(string itemId, int amount)
    {
        int removed = 0;

        for (int i = visualStack.Count - 1; i >= 0 && removed < amount; i--)
        {
            var e = visualStack[i];

            if (e == null || e.obj == null)
            {
                visualStack.RemoveAt(i);
                continue;
            }

            if (e.itemId != itemId)
                continue;

            GameObject obj = e.obj;
            visualStack.RemoveAt(i);
            Destroy(obj);
            removed++;
        }

        if (removed > 0)
            RepositionVisual();

        return removed;
    }

    private void RepositionVisual()
    {
        for (int i = 0; i < visualStack.Count; i++)
        {
            var e = visualStack[i];

            if (e == null || e.obj == null)
                continue;

            e.obj.transform.localPosition = Vector3.up * (offsetY * i);
            e.obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void SyncVisualFromSlots()
    {
        for (int i = visualStack.Count - 1; i >= 0; i--)
        {
            var e = visualStack[i];
            if (e != null && e.obj != null)
                Destroy(e.obj);
        }

        visualStack.Clear();

        if (!useHeadStackVisual)
            return;

        if (itemAssetManager == null)
            return;

        for (int i = 0; i < Slots.Count; i++)
        {
            var s = Slots[i];
            if (s.IsEmpty)
                continue;

            VisualAdd(s.itemId, s.count);
        }

        RepositionVisual();
    }
}