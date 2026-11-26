using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public Vector2Int gridSize;
    private Node[,] grid;

    void Awake()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        gridSize.x = bounds.size.x;
        gridSize.y = bounds.size.y;
        grid = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                bool isWalkable = !obstacleTilemap.HasTile(tilePos);
                Vector3 worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);

                grid[x, y] = new Node(isWalkable, worldPos, new Vector2Int(x, y));
            }
        }

        Test();
    }

    public Node GetNodeFromWorldPos(Vector3 worldPos)
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(worldPos);

        int x = cellPos.x - groundTilemap.cellBounds.xMin;
        int y = cellPos.y - groundTilemap.cellBounds.yMin;

        if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
        {
            return grid[x, y];
        }

        return null;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (groundTilemap == null) return;

        BoundsInt bounds = groundTilemap.cellBounds;

        Gizmos.color = Color.yellow;
        Vector3 centerPos = groundTilemap.GetCellCenterWorld(new Vector3Int((int)bounds.center.x, (int)bounds.center.y, (int)bounds.center.z));
        Gizmos.DrawWireCube(centerPos, bounds.size);

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
                Vector3 printPos = n.worldPos;
                printPos.z = -1f;
                Gizmos.DrawCube(printPos, Vector3.one * 0.3f);
            }
        }
    }

    void Test()
    {
        foreach (Node n in grid)
        {
            Debug.Log($"{n.worldPos}");
        }
    }
}