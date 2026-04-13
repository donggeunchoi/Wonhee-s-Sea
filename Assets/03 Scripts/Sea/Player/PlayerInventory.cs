using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> _fishList = new List<int>();

    public IReadOnlyList<int> FishList => _fishList;

    private float _maxWeight = 20f;

    public float MaxWeight => _maxWeight;
    public float CurrentWeight { get; private set; }

    public event Action<float, float> OnWeightChanged;

    private void Awake()
    {
        UserManager.Ins.OnFishID
            .Subscribe(AddFish)
            .AddTo(this);
    }

    private void Start()
    {
        OnWeightChanged?.Invoke(CurrentWeight, _maxWeight);
    }

    public void AddFish(int fishID)
    {
        var data = JsonManager.Ins.GetDataByKey<FishData>(fishID, x => x.fishId);

        if (data == null) return;

        if (CurrentWeight + data.weight > _maxWeight)
        {
            Debug.Log("무게가 꽉 찼습니다!");
            return;
        }

        _fishList.Add(fishID);
        CurrentWeight += data.weight;

        OnWeightChanged?.Invoke(CurrentWeight, _maxWeight);

        Debug.Log($"획득: {data.name} / 현재 무게: {CurrentWeight}");
    }

    public void LoseFishPercent(float percent)
    {
        if (_fishList.Count == 0) return;

        int loseCount = Mathf.FloorToInt(_fishList.Count * percent);

        for (int i = 0; i < loseCount; i++)
        {
            int index = UnityEngine.Random.Range(0, _fishList.Count);
            int fishID = _fishList[index];

            var data = JsonManager.Ins.GetDataByKey<FishData>(fishID, x => x.fishId);

            if (data != null)
                CurrentWeight -= data.weight;

            _fishList.RemoveAt(index);
        }

        CurrentWeight = Mathf.Max(0f, CurrentWeight);

        OnWeightChanged?.Invoke(CurrentWeight, _maxWeight);

        Debug.Log($"물고기 {loseCount}개 잃음 / 현재 무게: {CurrentWeight}");
    }
}
