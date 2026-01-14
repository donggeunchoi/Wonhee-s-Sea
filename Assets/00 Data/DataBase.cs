using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FishDB
{
    private readonly Dictionary<int, FishDataDef> _fishDict;

    public FishDB()
    {
        // JsonManager를 통해 리스트를 가져와서 딕셔너리로 변환
        var list = JsonManager.Ins.GetItems<FishDataDef>("FishData");
        _fishDict = list.ToDictionary(fish => fish.id);
        
        Debug.Log($"[FishDB] {_fishDict.Count}종의 물고기 데이터 로드 완료.");
    }

    public FishDataDef Get(int id)
    {
        if (_fishDict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[FishDB] ID {id}를 찾을 수 없습니다.");
        return null;
    }
}

public class SashimiDB
{
    private readonly Dictionary<int, SashimiDataDef> _SashimiDict;

    public SashimiDB()
    {
        var list = JsonManager.Ins.GetItems<SashimiDataDef>("SashimiData");
        _SashimiDict = list.ToDictionary(fish => fish.id);
        
        Debug.Log($"[FishDB] {_SashimiDict.Count}종의 물고기 데이터 로드 완료.");
    }

    public SashimiDataDef Get(int id)
    {
        if (_SashimiDict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[FishDB] ID {id}를 찾을 수 없습니다.");
        return null;
    }
}

public class SushiDB
{
    private readonly Dictionary<int, SushiDataDef> _SushiDict;

    public SushiDB()
    {
        var list = JsonManager.Ins.GetItems<SushiDataDef>("FishData");
        _SushiDict = list.ToDictionary(fish => fish.id);
        
        Debug.Log($"[FishDB] {_SushiDict.Count}종의 물고기 데이터 로드 완료.");
    }

    public SushiDataDef Get(int id)
    {
        if (_SushiDict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[FishDB] ID {id}를 찾을 수 없습니다.");
        return null;
    }
}

public class DpStandDB
{
    private readonly Dictionary<int, DpStandDataDef> _DpStandDict;

    public DpStandDB()
    {
        var list = JsonManager.Ins.GetItems<DpStandDataDef>("FishData");
        _DpStandDict = list.ToDictionary(fish => fish.id);
        
        Debug.Log($"[FishDB] {_DpStandDict.Count}종의 물고기 데이터 로드 완료.");
    }

    public DpStandDataDef Get(int id)
    {
        if (_DpStandDict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[FishDB] ID {id}를 찾을 수 없습니다.");
        return null;
    }
}

public class ItemChartDB
{
    private readonly Dictionary<int, ItemChartDef> _ItemChartDict;

    public ItemChartDB()
    {
        var list = JsonManager.Ins.GetItems<ItemChartDef>("FishData");
        _ItemChartDict = list.ToDictionary(fish => fish.itemId);
        
        Debug.Log($"[FishDB] {_ItemChartDict.Count}종의 물고기 데이터 로드 완료.");
    }

    public ItemChartDef Get(int id)
    {
        if (_ItemChartDict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[FishDB] ID {id}를 찾을 수 없습니다.");
        return null;
    }
}