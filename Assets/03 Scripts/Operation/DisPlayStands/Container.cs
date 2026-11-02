using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("보관 옵션")]
    [SerializeField]
    private int maxCapacity;

    [Header("아이콘 조회")]
    [SerializeField]
    private ItemAssetManager itemAssetManager;

    [Header("UI 프리팹")]
    [SerializeField]
    private FishSlotUI fishSlotPrefab;

    [Header("UI 생성 위치")]
    [SerializeField]
    private Transform content;

    [Header("캔버스 오브젝트")]
    [SerializeField]
    private GameObject visualRoot;

    [Header("플레이어 인벤토리")]
    [SerializeField]
    private Inventory playerInventory;

    [Header("아이템 데이터베이스")]
    [SerializeField]
    private ItemRuntimeDatabase itemRuntimeDatabase;

    private readonly Dictionary<string, FishSlotUI> fishSlots = new Dictionary<string, FishSlotUI>();

    private void Start()
    {
        InitializeItemDatabase();
        InitializeFishSlots();
        SubscribeFishTossEvent();
    }

    private void InitializeItemDatabase()
    {
        var items = JsonManager.Ins.GetJson<ItemDataTable>();

        if (items == null || items.Count == 0)
        {
            Debug.LogError("ItemDataTable 데이터가 없습니다.");
            return;
        }

        if (itemRuntimeDatabase == null)
        {
            Debug.LogError("ItemRuntimeDatabase가 연결되지 않았습니다.");
            return;
        }

        itemRuntimeDatabase.SetItems(items);
    }

    private void InitializeFishSlots()
    {
        ClearSlots();

        if (UserManager.Ins == null || UserManager.Ins.userInfo == null)
        {
            Debug.LogError("UserManager 또는 userInfo가 없습니다.");
            return;
        }

        Dictionary<string, int> fishContainer = UserManager.Ins.userInfo.fishContainer;

        if (fishContainer == null || fishContainer.Count == 0)
        {
            Debug.Log("컨테이너에 저장된 물고기가 없습니다.");
            return;
        }

        foreach (KeyValuePair<string, int> fishData in fishContainer)
        {
            string fishId = RemoveDungeonPrefix(fishData.Key);
            int fishCount = fishData.Value;

            if (string.IsNullOrEmpty(fishId) || fishCount <= 0)
                continue;

            CreateOrUpdateSlot(fishId, fishCount);
        }
    }

    private void SubscribeFishTossEvent()
    {
        if (BackendManager.Ins == null)
        {
            Debug.LogError("BackendManager가 없습니다.");
            return;
        }

        BackendManager.Ins.OnTossFish.AsObservable().Subscribe(OnFishTossed).AddTo(this);
    }

    private void OnFishTossed(string fishId)
    {
        if (string.IsNullOrEmpty(fishId))
            return;

        fishId = RemoveDungeonPrefix(fishId);

        int fishCount = UserManager.Ins.userInfo.GetFishCount(fishId);

        if (fishCount <= 0)
        {
            RemoveSlot(fishId);
            return;
        }

        CreateOrUpdateSlot(fishId, fishCount);
    }

    private void CreateOrUpdateSlot(string fishId, int fishCount)
    {
        if (fishSlotPrefab == null || content == null)
        {
            Debug.LogError("FishSlotUI 프리팹 또는 Content가 연결되지 않았습니다.");
            return;
        }

        if (itemAssetManager == null)
        {
            Debug.LogError("ItemAssetManager가 연결되지 않았습니다.");
            return;
        }

        Sprite fishSprite = itemAssetManager.GetIcon(fishId);

        if (fishSprite == null)
            Debug.LogWarning($"물고기 아이콘을 찾지 못했습니다. ID={fishId}");

        if (!fishSlots.TryGetValue(fishId, out FishSlotUI slot) || slot == null)
        {
            slot = Instantiate(fishSlotPrefab, content);
            fishSlots[fishId] = slot;
        }

        slot.FirstInit(fishSprite, fishId, fishCount, ReceiveFish);
    }

    private void ReceiveFish(string fishId)
    {
        if (string.IsNullOrEmpty(fishId))
        {
            Debug.LogWarning("물고기 ID가 비어있습니다.");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogWarning("Player Inventory가 연결되지 않았습니다.");
            return;
        }

        int fishCount = UserManager.Ins.userInfo.GetFishCount(fishId);

        if (fishCount <= 0)
        {
            Debug.LogWarning($"받을 물고기가 없습니다. ID={fishId}");
            RemoveSlot(fishId);
            return;
        }

        bool success = playerInventory.TryAdd(fishId, 1);

        if (!success)
        {
            Debug.LogWarning($"인벤토리에 물고기를 추가하지 못했습니다. ID={fishId}");
            return;
        }

        bool removed = UserManager.Ins.userInfo.RemoveFish(fishId, 1);

        if (!removed)
        {
            Debug.LogWarning($"컨테이너에서 물고기를 제거하지 못했습니다. ID={fishId}");
            return;
        }

        RefreshSlot(fishId);
        BackendManager.Ins.SaveData();
    }

    private void RefreshSlot(string fishId)
    {
        int fishCount = UserManager.Ins.userInfo.GetFishCount(fishId);

        if (fishCount <= 0)
        {
            RemoveSlot(fishId);
            return;
        }

        CreateOrUpdateSlot(fishId, fishCount);
    }

    private void RemoveSlot(string fishId)
    {
        if (!fishSlots.TryGetValue(fishId, out FishSlotUI slot))
            return;

        if (slot != null)
            Destroy(slot.gameObject);

        fishSlots.Remove(fishId);
    }

    private void ClearSlots()
    {
        foreach (KeyValuePair<string, FishSlotUI> fishSlot in fishSlots)
        {
            if (fishSlot.Value != null)
                Destroy(fishSlot.Value.gameObject);
        }

        fishSlots.Clear();
    }

    private string RemoveDungeonPrefix(string fishKey)
    {
        if (string.IsNullOrEmpty(fishKey))
            return string.Empty;

        if (fishKey.StartsWith("D", StringComparison.Ordinal))
            return fishKey.Substring(1);

        return fishKey;
    }

    public void Interact()
    {
        SetVisualActive(true);
    }

    public void SetVisualActive(bool active)
    {
        if (visualRoot != null)
            visualRoot.SetActive(active);
    }
}