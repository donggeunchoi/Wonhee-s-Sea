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
        InitializeSlots();
        SubscribeContainerEvent();
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

    private void InitializeSlots()
    {
        ClearSlots();

        if (UserManager.Ins == null || UserManager.Ins.userInfo == null)
        {
            Debug.LogError("UserManager 또는 userInfo가 없습니다.");
            return;
        }

        var userInfo = UserManager.Ins.userInfo;
        InitializeSlots( userInfo.fishContainer );
        InitializeSlots( userInfo.ingredientContainer );
    }

    private void InitializeSlots( Dictionary<string, int> items )
    {
        if( items == null )
            return;

        foreach( var item in items )
        {
            string itemId = RemoveDungeonPrefix( item.Key );
            if( item.Value <= 0 || GetContainer( itemId ) != items )
                continue;

            CreateOrUpdateSlot( itemId, item.Value );
        }
    }

    private void SubscribeContainerEvent()
    {
        if (BackendManager.Ins == null)
        {
            Debug.LogError("BackendManager가 없습니다.");
            return;
        }

        BackendManager.Ins.OnTossFish.AsObservable().Subscribe( OnContainerItemAdded ).AddTo(this);
    }

    private void OnContainerItemAdded( string itemId )
    {
        RefreshSlot( RemoveDungeonPrefix( itemId ) );
    }

    private Dictionary<string, int> GetContainer( string itemId )
    {
        var userInfo = UserManager.Ins?.userInfo;
        var data = itemRuntimeDatabase?.Get( itemId );
        if( userInfo == null || data == null )
            return null;

        if( data.itemType == EItemType.Fish )
            return userInfo.fishContainer;

        if( data.itemType == EItemType.Ingredient && data.ingredientType == EIngredientType.Rawrice )
            return userInfo.ingredientContainer;

        return null;
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
            Debug.LogWarning($"창고 아이템 아이콘을 찾지 못했습니다. ID={fishId}");

        if (!fishSlots.TryGetValue(fishId, out FishSlotUI slot) || slot == null)
        {
            slot = Instantiate(fishSlotPrefab, content);
            fishSlots[fishId] = slot;
        }

        slot.FirstInit(fishSprite, fishId, fishCount, ReceiveItem);
    }

    private void ReceiveItem( string itemId )
    {
        if( playerInventory == null )
        {
            Debug.LogWarning( "Player Inventory가 연결되지 않았습니다." );
            return;
        }

        var items = GetContainer( itemId );
        string key = "D" + itemId;
        if( items == null || items.TryGetValue( key, out int count ) == false || count <= 0 )
        {
            RemoveSlot( itemId );
            return;
        }

        if( count == 1 )
            items.Remove( key );
        else
            items[ key ] = count - 1;

        if( playerInventory.TryAdd( itemId, 1 ) == false )
        {
            items[ key ] = count;
            Debug.LogWarning( $"인벤토리에 창고 아이템을 추가하지 못했습니다. ID={itemId}" );
            return;
        }

        RefreshSlot( itemId );
        BackendManager.Ins.SaveData();
    }

    private void RefreshSlot( string itemId )
    {
        var items = GetContainer( itemId );
        if( items == null || items.TryGetValue( "D" + itemId, out int count ) == false || count <= 0 )
        {
            RemoveSlot( itemId );
            return;
        }

        CreateOrUpdateSlot( itemId, count );
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