
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerNPC : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float arriveDist = 0.1f;
    [SerializeField] private float waitTime = 3f;

    [Header("참조")]
    [SerializeField] private Inventory npcInventory;
    [SerializeField] private StorageStand targetStand;
    [SerializeField] private CheckoutStand checkout;
    [SerializeField] private Transform exitPoint;

    [Header("애니메이션")]
    [SerializeField] private Animator anim;

    private static readonly int HashIsMove =
        Animator.StringToHash("isMove");

    private static readonly int HashIsWait =
        Animator.StringToHash("isWait");

    // A* 길찾기
    private PathFinding pathfinding;
    private List<Node> currentPath;
    private int pathIndex;

    // 손님 상태
    private ENPCState state;

    // 코루틴
    private Coroutine moveCoroutine;
    private Coroutine waitCo;

    // 최종 목적지
    private Vector3 finalTargetPos;


    #region 초기화

    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (npcInventory == null)
            npcInventory = GetComponent<Inventory>();

        state = ENPCState.MoveToStand;

        if (targetStand != null)
        {
            // 매대는 기존 방식대로 생성 시 대기열 등록
            Vector3 queuePos = targetStand.JoinQueue(this);

            SetNewPath(queuePos);
        }
        else
        {
            GoExit();
        }

        RefreshAnim();
    }

    public void Init(
        StorageStand stand,
        CheckoutStand checkoutStand,
        Transform exit,
        Collider2D colstand,
        Collider2D colCheck,
        PathFinding pathfind)
    {
        targetStand = stand;
        checkout = checkoutStand;
        exitPoint = exit;
        pathfinding = pathfind;

        state = ENPCState.MoveToStand;
    }

    #endregion


    #region 애니메이션

    void RefreshAnim()
    {
        if (anim == null)
            return;

        bool isWait =
            state == ENPCState.Pick ||
            state == ENPCState.Pay;

        bool isMove =
            state == ENPCState.MoveToStand ||
            state == ENPCState.MoveToCheckout ||
            state == ENPCState.MoveToCheckoutQueue ||
            state == ENPCState.Exit;

        anim.SetBool(HashIsWait, isWait);
        anim.SetBool(HashIsMove, isMove);
    }

    private void UpdateDirection(Vector3 targetPos)
    {
        Vector3 scale = transform.localScale;

        if (targetPos.x > transform.position.x)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (targetPos.x < transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    #endregion


    #region 상품 선택

    void TryPickOne()
    {
        // TODO: 실제 재고 확인 및 아이템 획득 로직

        // 재고 없으면 퇴장
        // if (targetStand == null || !targetStand.HasStock(1))
        // {
        //     GoExit();
        //     return;
        // }

        // 매대 재고 차감
        // if (!targetStand.TryTake(1))
        // {
        //     GoExit();
        //     return;
        // }

        // 손님 인벤토리에 아이템 추가
        // string itemId = targetStand.SellItemId;
        // npcInventory?.TryAdd(itemId, 1);


        // 매대 대기열에서 제거
        if (targetStand != null)
        {
            targetStand.LeaveQueue(this);
        }

        if (checkout != null)
        {
            state = ENPCState.MoveToCheckout;

            // 실제 대기열이 아닌 이동 중인 손님으로 등록
            Vector3 targetPos =
                checkout.RegisterApproachingCustomer(this);

            SetNewPath(targetPos);
        }
        else
        {
            GoExit();
        }

        RefreshAnim();
    }

    private IEnumerator CoWaitThenPick()
    {
        // 첫 번째 순번이 될 때까지 대기
        while (!targetStand.IsFirstInLine(this))
        {
            yield return null;
        }

        // 상품 선택 시간
        yield return new WaitForSeconds(waitTime);

        waitCo = null;

        if (state == ENPCState.Pick)
        {
            TryPickOne();
        }
    }

    #endregion


    #region 계산 및 퇴장

    private IEnumerator CoWaitThenPay()
    {
        // 자신의 결제 순서까지 대기
        while (!checkout.IsFirstInLine(this))
        {
            yield return null;
        }

        // 결제 시간
        yield return new WaitForSeconds(waitTime);

        waitCo = null;

        if (state == ENPCState.Pay)
        {
            DoPay();
        }
    }

    private void DoPay()
    {
        if (targetStand == null || checkout == null)
        {
            GoExit();
            return;
        }

        string itemId = targetStand.SellItemId;

        // 손님 인벤토리에서 제거
        npcInventory?.TryRemove(itemId, 1);

        // 결제 및 골드 지급
        checkout.Pay(itemId, 1);

        // 출구로 이동
        GoExit();
    }

    private void GoExit()
    {
        StopWaiting();

        state = ENPCState.Exit;

        // 대기열에 남아 있는 경우 제거
        if (targetStand != null)
        {
            targetStand.LeaveQueue(this);
        }

        if (checkout != null)
        {
            checkout.LeaveQueue(this);
        }

        if (exitPoint != null)
        {
            SetNewPath(exitPoint.position);
        }
        else
        {
            Destroy(gameObject);
        }

        RefreshAnim();
    }

    #endregion


    #region 대기열 목적지 갱신

    // 매대에서 줄을 서고 있는 손님
    public void UpdateStandQueueTarget(Vector3 newTarget)
    {
        if (state != ENPCState.MoveToStand &&
            state != ENPCState.Pick)
        {
            return;
        }

        StopWaiting();

        state = ENPCState.MoveToStand;

        SetNewPath(newTarget);

        RefreshAnim();
    }

    // 계산대로 이동 중인 손님
    // 아직 실제 계산대 대기열에 등록되지 않은 상태
    public void UpdateCheckoutApproachTarget(Vector3 newTarget)
    {
        if (state != ENPCState.MoveToCheckout)
            return;

        // 동일한 목적지로 이동 중이면 재탐색 생략
        if (moveCoroutine != null &&
            Vector2.Distance(finalTargetPos, newTarget) < 0.01f)
        {
            return;
        }

        SetNewPath(newTarget);

        RefreshAnim();
    }

    // 이미 계산대에서 줄을 서고 있는 손님
    public void UpdateQueueTarget(Vector3 newTarget)
    {
        if (state != ENPCState.Pay &&
            state != ENPCState.MoveToCheckoutQueue)
        {
            return;
        }

        StopWaiting();

        state = ENPCState.MoveToCheckoutQueue;

        SetNewPath(newTarget);

        RefreshAnim();
    }

    private void StopWaiting()
    {
        if (waitCo != null)
        {
            StopCoroutine(waitCo);
            waitCo = null;
        }
    }

    #endregion


    #region A* 길찾기 및 이동

    void SetNewPath(Vector3 targetPos)
    {
        // 기존 이동 코루틴 종료
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        finalTargetPos = targetPos;
        pathIndex = 0;

        if (pathfinding == null)
        {
            Debug.LogError("[CustomerNPC] PathFinding이 없습니다.");
            return;
        }

        currentPath = pathfinding.FindPath(
            transform.position,
            targetPos
        );

        // 경로 탐색 실패
        if (currentPath == null)
        {
            Debug.LogWarning(
                $"[CustomerNPC] 경로 탐색 실패: {targetPos}"
            );

            return;
        }

        // 목적지가 동일한 그리드 내부에 있는 경우
        if (currentPath.Count == 0)
        {
            moveCoroutine = StartCoroutine(MoveToFinalTarget());
            return;
        }

        moveCoroutine = StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        yield return null;

        while (pathIndex < currentPath.Count)
        {
            Vector3 targetNodePos =
                currentPath[pathIndex].worldPos;

            // 마지막 노드는 실제 목적지로 교체
            if (pathIndex == currentPath.Count - 1)
            {
                targetNodePos = finalTargetPos;
            }

            targetNodePos.z = transform.position.z;

            // 이동 방향에 따른 스프라이트 반전
            UpdateDirection(targetNodePos);

            // 실제 이동
            while (Vector3.Distance(
                transform.position,
                targetNodePos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetNodePos,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetNodePos;

            pathIndex++;
        }

        moveCoroutine = null;

        OnReachedTarget();
    }

    private IEnumerator MoveToFinalTarget()
    {
        yield return null;

        Vector3 targetPos = finalTargetPos;
        targetPos.z = transform.position.z;

        UpdateDirection(targetPos);

        while (Vector3.Distance(
            transform.position,
            targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPos;

        moveCoroutine = null;

        OnReachedTarget();
    }

    #endregion


    #region 목적지 도착 및 상태 전환

    void OnReachedTarget()
    {
        switch (state)
        {
            // 매대 대기 위치 도착
            case ENPCState.MoveToStand:

                state = ENPCState.Pick;

                waitCo = StartCoroutine(CoWaitThenPick());

                break;


            // 계산대의 마지막 빈자리 도착
            case ENPCState.MoveToCheckout:

                if (checkout == null)
                {
                    GoExit();
                    return;
                }

                Vector3 tailPos =
                    checkout.GetQueueTailPosition();

                // 여전히 마지막 빈자리가 맞는지 확인
                if (Vector2.Distance(
                    transform.position,
                    tailPos) > arriveDist)
                {
                    // 대기열이 변경되었다면 다시 이동
                    SetNewPath(tailPos);

                    return;
                }

                // 실제 계산대 대기열에 등록
                checkout.JoinQueue(this);

                state = ENPCState.Pay;

                waitCo = StartCoroutine(CoWaitThenPay());

                break;


            // 계산대 대기열에서 앞으로 이동 완료
            case ENPCState.MoveToCheckoutQueue:

                state = ENPCState.Pay;

                waitCo = StartCoroutine(CoWaitThenPay());

                break;


            // 출구 도착
            case ENPCState.Exit:

                Destroy(gameObject);

                break;
        }

        RefreshAnim();
    }

    #endregion
}
