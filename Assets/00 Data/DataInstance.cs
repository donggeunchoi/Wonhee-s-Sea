using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInstance : Singleton<DataInstance>
{
    // 외부에서 접근할 때는 기존과 동일합니다.
    public FishDB Fishes { get; private set; }
    public SushiDB Sushis { get; private set; }
    public SashimiDB Sashimis { get; private set; }
    public StandDB Stands { get; private set; }           // 추가
    public EquipmentDB Equipments { get; private set; }   // 추가

    protected override void Awake()
    {
        base.Awake();
        
        // 싱글톤이므로 Awake에서 초기화하는 것이 안전합니다.
        // 만약 다른 스크립트의 Start에서 이 데이터들을 참조한다면, 
        // 여기서 미리 만들어둬야 NullReference 에러를 방지할 수 있습니다.
        InitializeDB();
    }

    private void InitializeDB()
    {
        Fishes = new FishDB();
        Sushis = new SushiDB();
        Sashimis = new SashimiDB();
        Stands = new StandDB();
        Equipments = new EquipmentDB();
        
        Debug.Log("[DataInstance] 모든 데이터베이스 초기화 완료.");
    }
}
