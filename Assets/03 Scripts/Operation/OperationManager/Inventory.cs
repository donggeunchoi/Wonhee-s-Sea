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
    [SerializeField] private ItemDatabaseSO dataBase;

    [Header("시작시 가지게 될 슬롯 개수(=수용량)")]
    [SerializeField] private int startSlot = 5;

    [Header("머리 위 기준점")]
    [SerializeField] private Transform stackRoot;

    [Header("스택 간격(프리팹 1개당 올라갈 높이)")]
    [SerializeField] private float offsetY = 0.3f;

    [Header("머리위에 표시할지 여부")]
    [SerializeField] private bool useHeadStackVisual = true;

    // (UI가 필요 없으면 이 이벤트/호출은 나중에 전부 삭제 가능)
    public event Action OnChange;

    public List<ItemStack> Slots { get; private set; } = new List<ItemStack>();

    //머리 위 비주얼: itemId를 같이 들고 있어야 “ID로 제거”가 가능
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

        // 시작할 때는 비주얼 동기화(원하면 지워도 됨)
        SyncVisualFromSlots();
        OnChange?.Invoke();
    }

    public void ExpandSlots(int add)
    {
        for (int i = 0; i < add; i++)
            Slots.Add(new ItemStack());

        // 수용량 확장만 했으면 비주얼 변경은 없음
        OnChange?.Invoke();
    }

    public int CountOf(string itemId)
    {
        int sum = 0;
        for (int i = 0; i < Slots.Count; i++)
            if (Slots[i].itemId == itemId) sum += Slots[i].count;
        return sum;
    }

    // -------------------------
    //Add/Remove 할 때 비주얼변경
    // -------------------------

    public bool TryAdd(string itemId, int amount)
    {
        if (amount <= 0) return true;
        if (dataBase == null) { Debug.LogError("Inventory: db 미할당"); return false; }

        // Add 전 수량
        int before = CountOf(itemId);

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

            if (!stackable && left > 0)
                continue;
        }

        if (left > 0) return false; // 공간 부족

        // Add 후 수량
        int after = CountOf(itemId);
        int added = after - before;

        if (useHeadStackVisual && added > 0)
            VisualAdd(itemId, added);

        OnChange?.Invoke();
        return true;
    }

    public bool TryRemove(string itemId, int amount)
    {
        if (amount <= 0) return true;

        int before = CountOf(itemId);
        if (before < amount) return false;

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

        int after = CountOf(itemId);
        int removed = before - after;

        if (useHeadStackVisual && removed > 0)
            VisualRemoveById(itemId, removed);

        OnChange?.Invoke();
        return true;
    }

    // -------------------------
    //머리 위 비주얼 로직
    // -------------------------

    private void VisualAdd(string itemId, int amount)
    {
        if (stackRoot == null)
        {
            Debug.LogWarning("Inventory: stackRoot 미할당");
            return;
        }

        // ItemSO 먼저 가져와서 원인 분리
        var itemSO = dataBase.Get(itemId);

        // 프리팹도 가져오기
        var prefab = dataBase.GetPrefab(itemId);

        // 진단 로그 (DB / ItemSO / Prefab 모두 확인)
        Debug.Log($"[INV] db={dataBase.name}, itemId='{itemId}', itemSO={(itemSO == null ? "NULL" : itemSO.name)}, prefab={(prefab == null ? "NULL" : prefab.name)}");

        if (itemSO == null)
        {
            Debug.LogError($"Inventory: ItemSO 없음 itemId={itemId}");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError($"Inventory: prefab 없음 itemId={itemId} (ItemSO: {itemSO.name})");
            return;
        }

        // amount 만큼 생성
        for (int i = 0; i < amount; i++)
        {
            var obj = Instantiate(prefab, stackRoot);
            obj.transform.localRotation = Quaternion.identity;

            visualStack.Add(new VisualEntry { itemId = itemId, obj = obj });
        }

        RepositionVisual();
    }

    private int VisualRemoveById(string itemId, int amount)
    {
        int removed = 0;

        // 뒤에서부터(최근 쌓인 것부터) 제거
        for (int i = visualStack.Count - 1; i >= 0 && removed < amount; i--)
        {
            var e = visualStack[i];
            if (e == null || e.obj == null)
            {
                visualStack.RemoveAt(i);
                continue;
            }

            if (e.itemId != itemId) continue;

            var obj = e.obj;
            visualStack.RemoveAt(i);
            Destroy(obj);
            removed++;
        }

        if (removed > 0) RepositionVisual();
        return removed;
    }

    private void RepositionVisual()
    {
        for (int i = 0; i < visualStack.Count; i++)
        {
            var e = visualStack[i];
            if (e == null || e.obj == null) continue;

            e.obj.transform.localPosition = Vector3.up * (offsetY * i);
        }
    }

    // “현재 Slots 상태”를 기준으로 비주얼을 완전 재구성 (디버그/초기화/세이브로드 후 유용)
    public void SyncVisualFromSlots()
    {
        // 기존 비주얼 제거
        for (int i = visualStack.Count - 1; i >= 0; i--)
        {
            var e = visualStack[i];
            if (e != null && e.obj != null) Destroy(e.obj);
        }
        visualStack.Clear();

        if (!useHeadStackVisual) return;
        if (dataBase == null) return;

        // Slots에서 itemId별 개수만큼 쌓기
        for (int i = 0; i < Slots.Count; i++)
        {
            var s = Slots[i];
            if (s.IsEmpty) continue;
            VisualAdd(s.itemId, s.count);
        }

        RepositionVisual();
    }
}
