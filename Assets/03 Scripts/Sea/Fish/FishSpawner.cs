using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private DepthCalculator _depthCalculator;

    [SerializeField] private List<FishRoot> _fishPrefabs = new List<FishRoot>();

    private float _rangeX = 8f;
    private float _rangeUp = 5f;
    private float _rangeDown = 10f;
    private float _innerSafeX = 5f;
    private float _rowHeight = 1f;
    private int _spawnPerRow = 1;
    private float _rowYOffsetRandom = 0.5f;
    private float _refreshDistance = 1.5f;
    private float _despawnMarginX = 4f;
    private float _despawnMarginY = 4f;
    private float _minSpawnDistance = 1.5f;

    private List<FishRoot> _activeFish = new List<FishRoot>();
    private Vector3 _lastPlayerPos;

    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("FishSpawner: _player가 비어있음", this);
            enabled = false;
            return;
        }

        if (_depthCalculator == null)
        {
            Debug.LogError("FishSpawner: _depthCalculator가 비어있음", this);
            enabled = false;
            return;
        }

        if (_fishPrefabs == null || _fishPrefabs.Count == 0)
        {
            Debug.LogError("FishSpawner: _fishPrefabs가 비어있음", this);
            enabled = false;
            return;
        }

        if (_innerSafeX >= _rangeX)
        {
            Debug.LogError("FishSpawner: _innerSafeX는 _rangeX보다 작아야 함", this);
            enabled = false;
            return;
        }

        _lastPlayerPos = _player.position;
        Refresh();
    }

    private void Update()
    {
        if (Vector2.Distance(_player.position, _lastPlayerPos) >= _refreshDistance)
        {
            _lastPlayerPos = _player.position;
            Refresh();
        }
    }

    private void Refresh()
    {
        RemoveNullFish();
        DespawnFarFish();
        SpawnByRows();
    }

    private void RemoveNullFish()
    {
        for (int i = _activeFish.Count - 1; i >= 0; i--)
        {
            if (_activeFish[i] == null)
                _activeFish.RemoveAt(i);
        }
    }

    private void DespawnFarFish()
    {
        float minX = _player.position.x - (_rangeX + _despawnMarginX);
        float maxX = _player.position.x + (_rangeX + _despawnMarginX);
        float minY = _player.position.y - (_rangeDown + _despawnMarginY);
        float maxY = _player.position.y + (_rangeUp + _despawnMarginY);

        for (int i = _activeFish.Count - 1; i >= 0; i--)
        {
            FishRoot fish = _activeFish[i];

            if (fish == null)
            {
                _activeFish.RemoveAt(i);
                continue;
            }

            Vector3 pos = fish.transform.position;

            bool isOut =
                pos.x < minX || pos.x > maxX ||
                pos.y < minY || pos.y > maxY;

            if (isOut)
            {
                _activeFish.RemoveAt(i);
                Destroy(fish.gameObject);
            }
        }
    }

    private void SpawnByRows()
    {
        float startY = _player.position.y + _rangeUp;
        int rowCount = Mathf.CeilToInt((_rangeUp + _rangeDown) / _rowHeight);

        for (int row = 0; row < rowCount; row++)
        {
            float rowTop = startY - (row * _rowHeight);
            float rowBottom = rowTop - _rowHeight;
            float rowCenterY = (rowTop + rowBottom) * 0.5f;

            int currentCount = CountFishInRow(rowTop, rowBottom);
            int needCount = _spawnPerRow - currentCount;

            if (needCount <= 0)
                continue;

            float depth = _depthCalculator.GetDepthByWorldY(rowCenterY);
            List<FishRoot> candidates = GetCandidatesByDepth(depth);

            if (candidates.Count == 0)
                continue;

            int tryCount = 0;
            int maxTryCount = needCount * 10;

            while (needCount > 0 && tryCount < maxTryCount)
            {
                tryCount++;

                Vector3 spawnPos = GetRandomPositionOutsideView(rowCenterY);

                if (IsTooCloseToOtherFish(spawnPos))
                    continue;

                FishRoot prefab = candidates[Random.Range(0, candidates.Count)];
                FishRoot newFish = Instantiate(prefab, spawnPos, Quaternion.identity);

                _activeFish.Add(newFish);
                needCount--;
            }
        }
    }

    private int CountFishInRow(float rowTop, float rowBottom)
    {
        int count = 0;

        for (int i = 0; i < _activeFish.Count; i++)
        {
            FishRoot fish = _activeFish[i];
            if (fish == null) continue;

            float y = fish.transform.position.y;

            if (y <= rowTop && y > rowBottom)
                count++;
        }

        return count;
    }

    private Vector3 GetRandomPositionOutsideView(float rowCenterY)
    {
        bool spawnLeft = Random.Range(0, 2) == 0;

        float x;

        if (spawnLeft)
            x = Random.Range(_player.position.x - _rangeX, _player.position.x - _innerSafeX);
        else
            x = Random.Range(_player.position.x + _innerSafeX, _player.position.x + _rangeX);

        float y = rowCenterY + Random.Range(-_rowYOffsetRandom, _rowYOffsetRandom);

        float maxY = _player.position.y + _rangeUp;
        float minY = _player.position.y - _rangeDown;
        y = Mathf.Clamp(y, minY, maxY);

        return new Vector3(x, y, 0f);
    }

    private bool IsTooCloseToOtherFish(Vector3 spawnPos)
    {
        float sqrDistance = _minSpawnDistance * _minSpawnDistance;

        for (int i = 0; i < _activeFish.Count; i++)
        {
            FishRoot fish = _activeFish[i];
            if (fish == null) continue;

            if ((fish.transform.position - spawnPos).sqrMagnitude < sqrDistance)
                return true;
        }

        return false;
    }

    private List<FishRoot> GetCandidatesByDepth(float depth)
    {
        List<FishRoot> candidates = new List<FishRoot>();

        for (int i = 0; i < _fishPrefabs.Count; i++)
        {
            FishRoot prefab = _fishPrefabs[i];
            if (prefab == null) continue;

            FishData data = JsonManager.Ins.GetDataByKey<FishData>(prefab.FishId, x => x.fishId);
            if (data == null) continue;

            if (depth >= data.minDepth && depth <= data.maxDepth)
                candidates.Add(prefab);
        }

        return candidates;
    }
}
