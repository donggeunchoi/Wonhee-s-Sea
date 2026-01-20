using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseDB<TKey, TData, TRaw>
{
    protected Dictionary<TKey, TData> _dict = new Dictionary<TKey, TData>();

    // 인덱서 property
    public TData this[TKey id] => Get(id);

    // 데이터 로드 공통 로직
    protected void Load(string fileName, System.Func<TRaw, TData> transform, System.Func<TData, TKey> keySelector)
    {
        // 1. 실제 파일 내용이 메모리에 들어오는지 확인
        var rawList = JsonManager.Ins.GetItems<TRaw>(fileName);

        if (rawList == null || rawList.Count == 0)
        {
            Debug.LogError($"<color=red>[파일 읽기 실패]</color> {fileName}.json을 찾지 못했거나 내용이 아예 없습니다.");
            return;
        }

        Debug.Log($"<color=cyan>[파일 확인]</color> {fileName}에서 {rawList.Count}개의 줄을 읽어옴.");

        _dict = new Dictionary<TKey, TData>();

        for (int i = 0; i < rawList.Count; i++)
        {
            try
            {
                TData data = transform(rawList[i]);
                TKey key = keySelector(data);

                // 헤더 무시
                if (key is int id && id <= 0) continue;

                if (_dict.ContainsKey(key)) continue;

                _dict.Add(key, data);
            }
            catch (System.Exception e)
            {
                // [중요] 여기서 터지는 이유가 나옵니다.
                Debug.LogError($"<color=yellow>[변환 에러]</color> {fileName}의 {i}번째 줄에서 에러 발생: {e.Message}\n원문: {rawList[i].ToString()}");
            }
        }

        Debug.Log($"<color=green>[최종 완료]</color> {this.GetType().Name}: {_dict.Count}개 로드 성공.");
    }
    public TData Get(TKey id)
    {
        if (_dict.TryGetValue(id, out var def)) return def;
        Debug.LogError($"[{this.GetType().Name}] ID {id}를 찾을 수 없습니다.");
        return default;
    }
}

public class FishDB : BaseDB<int, FishDataDef, FishRaw>
{
    public FishDB() => Load("Fish_Data", raw => new FishDataDef(raw), data => data.fishId);
}

public class SashimiDB : BaseDB<int, SashimiDataDef, SashimiRaw>
{
    public SashimiDB() => Load("Sashimi_Data", raw => new SashimiDataDef(raw), data => data.sashimiId);
}

public class SushiDB : BaseDB<int, SushiDataDef, SushiRaw>
{
    public SushiDB() => Load("Sushi_Data", raw => new SushiDataDef(raw), data => data.sushiId);
}

public class StandDB : BaseDB<int, StandDataDef, StandRaw>
{
    public StandDB() => Load("Stand_Data", raw => new StandDataDef(raw), data => data.standId);
}

public class EquipmentDB : BaseDB<int, EquipmentDataDef, EquipmentRaw>
{
    public EquipmentDB() => Load("Equipment_Data", raw => new EquipmentDataDef(raw), data => data.equipmentId);
}