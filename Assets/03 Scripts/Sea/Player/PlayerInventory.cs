using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> _fishList = new List<int>();

    public IReadOnlyList<int> FishList => _fishList;

    public void AddFish(int fishID)
    {
        _fishList.Add(fishID);
        Debug.Log($"물고기 획득 : {fishID} ");
    }

    public void LoseFishPercent(float percent)
    {
        if (_fishList.Count == 0) return;

        int loseCount = Mathf.FloorToInt(_fishList.Count * percent);

        for (int i = 0; i < loseCount; i++)
        {
            int index = Random.Range(0, _fishList.Count);
            _fishList.RemoveAt(index);
        }

        Debug.Log($"물고기 {loseCount}개 잃음");
    }
}
