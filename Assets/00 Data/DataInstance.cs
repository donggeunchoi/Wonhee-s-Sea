using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInstance : Singleton<DataInstance>
{
    // 내부 변수들
    private FishDB _fishes;
    private SushiDB _sushis;
    private SashimiDB _sashimis;
    private StandDB _stands;
    private EquipmentDB _equipments;

    // 프로퍼티 접근 시 null 체크 후 생성
    public FishDB Fishes => _fishes ??= new FishDB();
    public SushiDB Sushis => _sushis ??= new SushiDB();
    public SashimiDB Sashimis => _sashimis ??= new SashimiDB();
    public StandDB Stands => _stands ??= new StandDB();
    public EquipmentDB Equipments => _equipments ??= new EquipmentDB();

    public void Find_Id_Group(int id)
    {
        if (Fishes == null || Stands == null)
        {
            Debug.LogError("DB가 아직 초기화되지 않았습니다! InitializeDB가 호출됐는지 확인하세요.");
            return;
        }

        if (TryGetFishes(id, out var fishData))
        {
            Debug.Log($"{fishData.Name}");
        }
        else if (TryGetSushis(id, out var sushiData))
        {
            Debug.Log($"{sushiData.Name}");
        }
        else if (TryGetSashimis(id, out var sashimiData))
        {
            Debug.Log($"{sashimiData.Name}");
        }
        else if (TryGetStands(id, out var standData))
        {
            Debug.Log($"{standData.Name}");
        }
        else if (TryGetEquipments(id, out var equipmentData))
        {
            Debug.Log($"{equipmentData.Name}");
        }
    }

    public bool TryGetFishes(int id, out FishDataDef fish) => Fishes.TryGetValue(id, out fish);
    public bool TryGetSushis(int id, out SushiDataDef sushi) => Sushis.TryGetValue(id, out sushi);
    public bool TryGetSashimis(int id, out SashimiDataDef sashimi) => Sashimis.TryGetValue(id, out sashimi);
    public bool TryGetStands(int id, out StandDataDef stand) => Stands.TryGetValue(id, out stand);
    public bool TryGetEquipments(int id, out EquipmentDataDef equipment) => Equipments.TryGetValue(id, out equipment);
    
}
