using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [Header("물고기 프리팹")]
    [SerializeField] private GameObject _fishA; // 수심1~3m
    [SerializeField] private GameObject _fishB; // 수심4~6m
    [SerializeField] private GameObject _fishC; // 수심7~9m

    private void Start()
    {
        Spawn(_fishA, 1, 3);
        Spawn(_fishB, 4, 6);
        Spawn(_fishC, 7, 9);
    }

    private void Spawn(GameObject prefab, float minDepth, float maxDepth)
    {
        if (prefab == null) return;

        float spawnX = _player.position.x + Random.Range(-4f, 4f);

        float randomDepth = Random.Range(minDepth, maxDepth);
        float spawnY = 4.5f - randomDepth;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
