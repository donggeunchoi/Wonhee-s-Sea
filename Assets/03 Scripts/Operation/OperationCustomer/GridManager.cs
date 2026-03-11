using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{   
    public Tilemap obstacleTilemap;
    public Vector2Int gridSize;
    private Node[,] grid;

    void Awake()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                bool isWalkable = !obstacleTilemap.HasTile(tilePos);
                Vector3 wordPos = obstacleTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
                grid[x, y] = new Node(isWalkable, wordPos, new Vector2Int(x, y));
            }
        }
    }

    public Node GetNodeFromWorldPos(Vector3 worldPos)
    {
        Vector3Int cellPos = obstacleTilemap.WorldToCell(worldPos);
        return grid[cellPos.x, cellPos.y];
    }
}
