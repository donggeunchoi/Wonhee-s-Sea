using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindDestination : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Node
{
    public bool walkable;
    public Vector2Int gridPos;
    public Vector3 worldPos;

    public int gCost; // 시작점에서 노드까지의 거리
    public int hCost; // 목적지까지의 예상거리
    public Node parent;

    public int fCost => gCost + hCost; // 총 점수

    public Node(bool _walkable, Vector3 _worldPos, Vector2 _gridPos)
    {
        _walkable = walkable;
        _worldPos = worldPos;
        _gridPos = gridPos;
    }

}
