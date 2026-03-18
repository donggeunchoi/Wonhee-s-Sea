using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustmoerAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public PathFinding pathFinding;
    List<Node> currentPath;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            SetDestination(mousePos);
        }
    }

    public void SetDestination(Vector3 targetWorldPos)
    {
        currentPath = pathFinding.FindPath(transform.position, targetWorldPos);

        if (currentPath != null && currentPath.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        int targetIndex = 0;

        while (targetIndex < currentPath.Count)
        {
            Vector3 targetPos = currentPath[targetIndex].worldPos;
            targetPos.z = transform.position.z;

            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            targetIndex++;
        }

        Debug.Log("목적지 도착");
    }
}
