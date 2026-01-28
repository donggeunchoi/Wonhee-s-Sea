using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemDatabase")]
public class ItemDatabaseSO : ScriptableObject
{
    [SerializeField] private List<ItemSO> items = new();

    private Dictionary<string, ItemSO> _map;

    private void Build()
    {
        _map = new Dictionary<string, ItemSO>();
        foreach (var it in items)
        {
            if (it == null || string.IsNullOrEmpty(it.itemId)) continue;
            _map[it.itemId] = it;
        }
    }

    public ItemSO Get(string itemId)
    {
        if (_map == null) Build();
        if (string.IsNullOrEmpty(itemId)) return null;
        _map.TryGetValue(itemId, out var data);
        return data;
    }

    public GameObject GetPrefab(string itemId)
    {
        var data = Get(itemId);

        if(data == null) return null;

        if(data.prefab == null)
        {
            Debug.LogWarning($"ItemDatabaseSO: 아이템 {itemId} 의 프리팹이 설정되지 않음");
            return null;
        }

        return data.prefab;
    }

    public Sprite GetIcon(string itemId) => Get(itemId)?.icon;
    public int GetMaxStack(string itemId) => Get(itemId)?.maxStack ?? 99;
    public bool IsStackable(string itemId) => Get(itemId)?.stackable ?? true;

    private void OnEnable() => Build();
    private void OnValidate() => Build();
}