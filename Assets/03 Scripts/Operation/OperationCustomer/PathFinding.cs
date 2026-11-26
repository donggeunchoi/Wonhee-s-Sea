using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GridManager gridManager;

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 월드 좌표를 그리드 노드로 변환
        Node startNode = gridManager.GetNodeFromWorldPos(startPos);
        Node targetNode = gridManager.GetNodeFromWorldPos(targetPos);

        // openSet : 검사 예정 / closedSet : 검사 완료
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // 길찾기 시작
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // 오픈 리스트 (후보군)에서 fCost가 가장 낮은 노드 찾기
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            // 검사한 노드를 오픈 리스트에서 지우고 클로즈리스트에 추가
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 현재 노드가 목표 노드라면 역추적해 Path 반환
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // 이웃 노드들 검사
            foreach (Node neighbor in gridManager.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                // 이웃으로 가는 비용 계산
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // 검사한 적 없는 노드거나 다른 길로 왔을 때 값이 더 낮아진 노드라면 새롭게 기록
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // 역순으로 담긴 리스트 리버스 시켜주기
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int dstY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
