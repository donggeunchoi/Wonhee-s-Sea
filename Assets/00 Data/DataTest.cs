using UnityEngine;

public class DataTest : MonoBehaviour
{
    void Start()
    {
        var mackeral = DataInstance.Ins.Stands[40000];
        if (mackeral != null)
        {
            Debug.Log($"[성공] : {mackeral.standId}");
        }

        var shark = DataInstance.Ins.Sushis[30002];
        if (shark != null)
        {
            Debug.Log($"[성공] {shark.sushiId}");
        }
    }
}
