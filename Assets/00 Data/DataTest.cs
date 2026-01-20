using UnityEngine;

public class DataTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("--- 데이터 로드 테스트 시작 ---");

        var mackeral = DataInstance.Ins.Sushis[30001];
        if (mackeral != null)
        {
            Debug.Log($"[성공] 이름: {mackeral.sushiId}");
        }

        var shark = DataInstance.Ins.Sushis[30002];
        if (shark != null)
        {
            Debug.Log($"[성공] {shark.sushiId}");
        }

        var none = DataInstance.Ins.Sushis[999];
        if (none == null)
        {
            Debug.Log("[성공] 존재하지 않는 데이터에 대해 null을 반환함.");
        }
    }
}
