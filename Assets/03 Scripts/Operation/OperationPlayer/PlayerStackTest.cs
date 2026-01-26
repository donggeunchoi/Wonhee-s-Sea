using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStackTest : MonoBehaviour
{
    [SerializeField] private PlayerStackVisualizer stack;
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private GameObject testPrefab2;
    [SerializeField] private int addCount = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (stack == null || testPrefab == null)
            {
                Debug.LogWarning("Stack 또는 Prefab 연결 안됨");
                return;
            }

            for (int i = 0; i < addCount; i++)
            {
                stack.Add(testPrefab);
                stack.Add(testPrefab2);
            }
        }
    }
}
