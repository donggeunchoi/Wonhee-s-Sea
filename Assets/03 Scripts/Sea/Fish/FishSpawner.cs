using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private DepthCalculator _depthCalculator;

    [SerializeField] private List<FishRoot> _fishPrefabs = new List<FishRoot>();

    private float _rangeX = 10f;           // 좌우 스폰 범위
    private float _rangeUp = 6f;           // 위쪽 (화면 위)
    private float _rangeDown = 12f;        // 아래쪽 (화면 아래)
    private float _innerSafeX = 4f;        // 플레이어 근처 안전거리
    private float _rowHeight = 2.5f;       // row 간격 (조금 넓게)
    private int _spawnPerRow = 2;          // 한 row당 목표 마리수
    private float _rowYOffsetRandom = 1f;
    private float _refreshDistance = 2f;   // 플레이어가 이 거리만큼 움직이면 Refresh
    private float _minSpawnDistance = 2f;  // 물고기끼리 최소 거리

    private List<FishRoot> _activeFish = new List<FishRoot>();
    private HashSet<int> _spawnFishIds = new HashSet<int>();
    private bool _useSpawnFishFilter;
    private Vector3 _lastPlayerPos;

    private void Start()
    {
        if (_player == null || _depthCalculator == null || _fishPrefabs.Count == 0)
        {
            Debug.LogError("FishSpawner 설정 오류", this);
            enabled = false;
            return;
        }

        CacheSpawnFishIds();
        _lastPlayerPos = _player.position;
        Refresh(); // 초기 스폰
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
        float minX = _player.position.x - (_rangeX + 5f);
        float maxX = _player.position.x + (_rangeX + 5f);
        float minY = _player.position.y - (_rangeDown + 6f);
        float maxY = _player.position.y + (_rangeUp + 6f);

        for (int i = _activeFish.Count - 1; i >= 0; i--)
        {
            FishRoot fish = _activeFish[i];
            if (fish == null)
            {
                _activeFish.RemoveAt(i);
                continue;
            }

            Vector3 pos = fish.transform.position;

            if (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY)
            {
                _activeFish.RemoveAt(i);
                Destroy(fish.gameObject);
            }
        }
    }

    private void SpawnByRows()
    {
        float startY = _player.position.y + _rangeUp;
        int rowCount = Mathf.CeilToInt((_rangeUp + _rangeDown) / _rowHeight) + 1; // +1 여유

        for (int row = 0; row < rowCount; row++)
        {
            float rowCenterY = startY - (row * _rowHeight) - (_rowHeight * 0.5f);

            int currentCount = CountFishInRow(rowCenterY - _rowHeight * 0.6f, rowCenterY + _rowHeight * 0.6f);
            int needCount = _spawnPerRow - currentCount;

            if (needCount <= 0) continue;

            float depth = _depthCalculator.GetDepthByWorldY(rowCenterY);
            List<FishRoot> candidates = GetCandidatesByDepth(depth);

            if (candidates.Count == 0) continue;

            for (int i = 0; i < needCount; i++)
            {
                int tryCount = 0;
                const int maxTry = 20;

                while (tryCount < maxTry)
                {
                    tryCount++;
                    Vector3 spawnPos = GetRandomSpawnPosition(rowCenterY);

                    if (!IsTooCloseToOtherFish(spawnPos))
                    {
                        FishRoot prefab = candidates[Random.Range(0, candidates.Count)];
                        FishRoot newFish = Instantiate(prefab, spawnPos, Quaternion.identity);
                        _activeFish.Add(newFish);
                        break;
                    }
                }
            }
        }
    }

    private int CountFishInRow(float minY, float maxY)
    {
        int count = 0;
        for (int i = 0; i < _activeFish.Count; i++)
        {
            if (_activeFish[i] == null) continue;
            float y = _activeFish[i].transform.position.y;
            if (y >= minY && y <= maxY)
                count++;
        }
        return count;
    }

    private Vector3 GetRandomSpawnPosition(float targetY)
    {
        bool spawnLeft = Random.Range(0, 2) == 0;

        float x = spawnLeft ?
            Random.Range(_player.position.x - _rangeX, _player.position.x - _innerSafeX) :
            Random.Range(_player.position.x + _innerSafeX, _player.position.x + _rangeX);

        float y = targetY + Random.Range(-_rowYOffsetRandom, _rowYOffsetRandom);

        // Y 범위 제한
        y = Mathf.Clamp(y, _player.position.y - _rangeDown, _player.position.y + _rangeUp);

        return new Vector3(x, y, 0f);
    }

    private bool IsTooCloseToOtherFish(Vector3 spawnPos)
    {
        float sqrDist = _minSpawnDistance * _minSpawnDistance;
        for (int i = 0; i < _activeFish.Count; i++)
        {
            if (_activeFish[i] == null) continue;
            if ((_activeFish[i].transform.position - spawnPos).sqrMagnitude < sqrDist)
                return true;
        }
        return false;
    }

    private List<FishRoot> GetCandidatesByDepth(float depth)
    {
        List<FishRoot> candidates = new List<FishRoot>();

        foreach (FishRoot prefab in _fishPrefabs)
        {
            if (prefab == null) continue;
            if (_useSpawnFishFilter && !_spawnFishIds.Contains(prefab.FishId)) continue;

            FishData data = JsonManager.Ins.GetDataByKey<FishData>(prefab.FishId, x => x.fishId);
            if (data != null && depth >= data.minDepth && depth <= data.maxDepth)
                candidates.Add(prefab);
        }

        return candidates;
    }

    private void CacheSpawnFishIds()
    {
        _spawnFishIds.Clear();
        _useSpawnFishFilter = false;

        if (UserManager.Ins == null || UserManager.Ins.dungeonSpawnFishIds == null)
            return;

        _useSpawnFishFilter = true;

        for (int i = 0; i < UserManager.Ins.dungeonSpawnFishIds.Count; i++)
        {
            int fishId = UserManager.Ins.dungeonSpawnFishIds[i];
            if (fishId > 0)
                _spawnFishIds.Add(fishId);
        }
    }
}
