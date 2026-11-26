using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private DepthCalculator _depthCalculator;
    [SerializeField] private List<FishRoot> _fishPrefabs = new List<FishRoot>();

    [SerializeField] private List<SpeciesSpawnOverride> _speciesSpawnOverrides = new List<SpeciesSpawnOverride>();
    [SerializeField, Min(1)] private int _maxAlivePerSpecies = 6;
    [SerializeField, Min(0.1f)] private float _refreshInterval = 2f;
    [SerializeField] private Vector2 _horizontalBounds = new Vector2(-10f, 10f);
    [SerializeField] private Vector2 _depthBounds = new Vector2(0f, 100f);
    [SerializeField, Min(1)] private int _activationsPerFrame = 2;

    private const float ActivationMargin = 5f;
    private const float DeactivationMargin = 9f;
    private const float SafeDistance = 4f;

    [System.Serializable]
    private sealed class SpeciesSpawnOverride
    {
        public FishRoot Prefab;
        [Min(0)] public int MaxAlive;
        [Min(0f)] public float RespawnDelay;
    }

    private sealed class FishSlot
    {
        public FishRoot Prefab;
        public float MinY;
        public float MaxY;
        public Vector3 Position;
        public FishRoot Instance;
        public bool Alive;
        public float RespawnTime;
        public float RespawnDelay;
    }

    private readonly List<FishSlot> _slots = new List<FishSlot>();
    private Camera _camera;

    private void Start()
    {
        if (_player == null || _depthCalculator == null || JsonManager.Ins == null)
        {
            Debug.LogError("FishSpawner: missing player, depth calculator, or fish data.", this);
            enabled = false;
            return;
        }

        _camera = Camera.main;
        var allowedIds = UserManager.Ins?.dungeonSpawnFishIds;
        var registeredIds = new HashSet<int>();

        foreach (FishRoot prefab in _fishPrefabs)
        {
            if (prefab == null || (allowedIds != null && allowedIds.Count > 0 && !allowedIds.Contains(prefab.FishId)))
                continue;

            FishData data = JsonManager.Ins.GetDataByKey<FishData>(prefab.FishId, fish => fish.fishId);
            if (data == null || !registeredIds.Add(prefab.FishId))
                continue;

            float minDepth = Mathf.Max(0f, Mathf.Max(_depthBounds.x, data.minDepth));
            float maxDepth = Mathf.Min(_depthBounds.y, data.maxDepth);
            if (minDepth > maxDepth)
                continue;

            SpeciesSpawnOverride spawnOverride = _speciesSpawnOverrides.Find(entry => entry != null && entry.Prefab == prefab);
            int maxAlive = spawnOverride != null && spawnOverride.MaxAlive > 0 ? spawnOverride.MaxAlive : _maxAlivePerSpecies;
            float respawnDelay = spawnOverride != null && spawnOverride.RespawnDelay > 0f ? spawnOverride.RespawnDelay : _refreshInterval;
            for (int i = 0; i < maxAlive; i++)
            {
                var slot = new FishSlot
                {
                    Prefab = prefab,
                    RespawnDelay = respawnDelay,
                    MinY = _depthCalculator.SurfaceY - maxDepth,
                    MaxY = _depthCalculator.SurfaceY - minDepth
                };
                slot.Position = RandomPosition(slot);
                slot.Alive = true;
                _slots.Add(slot);
            }
        }
    }

    private void LateUpdate()
    {
        if (Time.timeScale <= 0f || _player == null)
            return;

        Vector2 center = _camera != null ? _camera.transform.position : _player.position;
        float halfHeight = _camera != null && _camera.orthographic ? _camera.orthographicSize : 6f;
        float halfWidth = _camera != null ? halfHeight * _camera.aspect : 10f;
        var view = new Vector2(halfWidth, halfHeight);
        int activationsLeft = _activationsPerFrame;

        foreach (FishSlot slot in _slots)
        {
            if (!slot.Alive && Time.time >= slot.RespawnTime)
                TryRespawn(slot, center, view);

            if (!slot.Alive)
                continue;

            if (slot.Instance != null && slot.Instance.gameObject.activeSelf)
            {
                slot.Position = slot.Instance.transform.position;
                if (!IsWithinView(slot.Position, center, view, DeactivationMargin))
                {
                    slot.Instance.HpUI?.Hide();
                    slot.Instance.gameObject.SetActive(false);
                }
                continue;
            }

            if (activationsLeft <= 0 || !IsWithinView(slot.Position, center, view, ActivationMargin))
                continue;

            Activate(slot);
            activationsLeft--;
        }
    }

    private void Activate(FishSlot slot)
    {
        if (slot.Instance == null)
        {
            slot.Instance = Instantiate(slot.Prefab, slot.Position, Quaternion.identity, transform);
            slot.Instance.Stats.SetDeathHandler(() => Release(slot));
            return;
        }

        if (slot.Instance.Stats.IsDead)
            ResetForRespawn(slot.Instance, slot.Position);

        slot.Instance.gameObject.SetActive(true);
    }

    private static void ResetForRespawn(FishRoot fish, Vector3 position)
    {
        fish.transform.position = position;
        if (fish.Rb != null)
        {
            fish.Rb.position = position;
            fish.Rb.velocity = Vector2.zero;
            fish.Rb.angularVelocity = 0f;
        }

        fish.Stats.Init(fish);
        fish.Sensor.Init(fish);
        fish.Controller.ResetForSpawn();
        fish.HpUI?.Init(fish.Stats);
    }

    private void Release(FishSlot slot)
    {
        slot.Alive = false;
        slot.RespawnTime = Time.time + slot.RespawnDelay;
        slot.Instance.gameObject.SetActive(false);
    }

    private void TryRespawn(FishSlot slot, Vector2 center, Vector2 view)
    {
        for (int attempt = 0; attempt < 8; attempt++)
        {
            Vector3 position = RandomPosition(slot);
            if (IsWithinView(position, center, view, 0f) ||
                ((Vector2)(position - _player.position)).sqrMagnitude < SafeDistance * SafeDistance)
                continue;

            slot.Position = position;
            slot.Alive = true;
            return;
        }

        slot.RespawnTime = Time.time + slot.RespawnDelay;
    }

    private Vector3 RandomPosition(FishSlot slot)
    {
        return new Vector3(Random.Range(_horizontalBounds.x, _horizontalBounds.y),
            Random.Range(slot.MinY, slot.MaxY), 0f);
    }

    private static bool IsWithinView(Vector2 position, Vector2 center, Vector2 view, float margin)
    {
        Vector2 offset = position - center;
        return Mathf.Abs(offset.x) <= view.x + margin && Mathf.Abs(offset.y) <= view.y + margin;
    }
}
