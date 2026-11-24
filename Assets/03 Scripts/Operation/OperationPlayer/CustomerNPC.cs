using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class CustomerNPC : MonoBehaviour
{
    [Header("이동")]
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float arriveDist = 0.1f;
    [SerializeField]
    private Collider2D colStorage;
    [SerializeField]
    private Collider2D colChechOut;
    [SerializeField]
    private float waitTime = 3f;


    [Header("참조")]
    [SerializeField]
    private Inventory npcInventory;
    [SerializeField]
    private StorageStand targetStand;
    [SerializeField]
    private CheckoutStand checkout;
    [SerializeField]
    private Transform exitPoint;

    [Header("애니메이션")]
    [SerializeField]
    Animator anim;
    private static readonly int HashIsMove = Animator.StringToHash("isMove");
    private static readonly int HashIsWait = Animator.StringToHash("isWait");

    [Header("A* 길찾기 참조")]
    private PathFinding pathfinding;
    private List<Node> currentPath;
    private int pathIndex = 0;
    private Coroutine moveCoroutine;

    private ENPCState state;
    private Transform currentTarget;

    private Coroutine waitCo;
    private Vector3 finalTargetPos;

    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        state = ENPCState.MoveToStand;

        if (targetStand != null)
        {
            // 매대 대기열에 등록하고 자신의 대기 위치 받기
            Vector3 queuePos = targetStand.JoinQueue(this);

            SetNewPath(queuePos);
        }

        RefreshAnim();
    }

    void RefreshAnim()
    {
        if (anim == null) return;

        bool isWait = (state == ENPCState.Pick || state == ENPCState.Pay);

        bool isMove = (state == ENPCState.MoveToStand || state == ENPCState.MoveToCheckout || state == ENPCState.Exit);

        anim.SetBool(HashIsWait, isWait);
        anim.SetBool(HashIsMove, isMove);
    }
    // Update is called once per frame
    void Update()
    {
        RefreshAnim();
    }

    void MoveTo()
    {
        if (currentTarget == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        if (state == ENPCState.Exit)
        {
            if (Vector3.Distance(transform.position, currentTarget.position) <= 0.05f)
            {
                Destroy(gameObject);
            }
        }
        RefreshAnim();
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (waitCo != null) return;

    //     // 1) 매대 트리거 들어옴 → Pick
    //     if (state == ENPCState.MoveToStand && targetStand != null && other.transform == colStorage.transform)
    //     {
    //         state = ENPCState.Pick;
    //         RefreshAnim();
    //         waitCo = StartCoroutine(CoWaitThenPick());
    //         return;
    //     }

    //     // 2) 계산대 트리거 들어옴 → Pay
    //     if (state == ENPCState.MoveToCheckout && checkout != null && other.transform == colChechOut.transform)
    //     {
    //         state = ENPCState.Pay;
    //         RefreshAnim();
    //         waitCo = StartCoroutine(CoWaitThenPay());
    //         return;
    //     }
    // }

    void TryPickOne()
    {
        // // 재고 없으면 Exit
        // if (targetStand == null || !targetStand.HasStock(1))
        // {
        //     GoExit();
        //     return;
        // }

        // // 꺼내기 실패하면 Exit
        // if (!targetStand.TryTake(1))
        // {
        //     GoExit();
        //     return;
        // }

        // // 성공하면 인벤 추가
        // string itemId = targetStand.SellItemId;
        // npcInventory?.TryAdd(itemId, 1);

        // 매대 대기열에서 제거
        if (targetStand != null)
        {
            targetStand.LeaveQueue(this);
        }

        // 계산대로 이동
        if (checkout != null)
        {
            state = ENPCState.MoveToCheckout;

            Vector3 queuePos = checkout.JoinQueue(this);

            SetNewPath(queuePos);
        }
        else
        {
            GoExit();
        }

        RefreshAnim();

    }

    private void GoExit()
    {
        state = ENPCState.Exit;
        SetNewPath(exitPoint.position);
        RefreshAnim();
    }


    private void DoPay()
    {
        if (targetStand == null || checkout == null)
        {
            GoExit();
            return;
        }

        string itemId = targetStand.SellItemId;

        // 손님 인벤에서 제거
        npcInventory?.TryRemove(itemId, 1);

        // 결제 처리
        checkout.Pay(itemId, 1);

        checkout.LeaveQueue(this);

        GoExit();
        RefreshAnim();
    }

    private IEnumerator CoWaitThenPick()
    {
        // 자신의 순번이 될 때까지 대기
        while (!targetStand.IsFirstInLine(this))
        {
            yield return null;
        }

        // 첫 번째 순번이 된 상태에서 구매 시간만큼 대기
        yield return new WaitForSeconds(waitTime);

        waitCo = null;

        if (state == ENPCState.Pick)
        {
            TryPickOne();
        }
    }

    private IEnumerator CoWaitThenPay()
    {
        while (!checkout.IsFirstInLine(this))
        {
            state = ENPCState.Pay;
            RefreshAnim();
            yield return null;
        }

        yield return new WaitForSeconds(waitTime);

        waitCo = null;

        if (state == ENPCState.Pay)
            DoPay();
    }


    // 외부에서 세팅용
    public void Init(StorageStand stand, CheckoutStand checkoutStand, Transform exit, Collider2D colstnad, Collider2D colCheck, PathFinding pathfind)
    {
        targetStand = stand;
        checkout = checkoutStand;
        exitPoint = exit;
        colStorage = colstnad;
        colChechOut = colCheck;
        pathfinding = pathfind;

        state = ENPCState.MoveToStand;

        if (targetStand != null)
        {
            currentTarget = targetStand.CustomerWaitPoint != null
                ? targetStand.CustomerWaitPoint
                : targetStand.transform;
        }

        RefreshAnim();
    }

    void SetNewPath(Vector3 targetPos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        finalTargetPos = targetPos;

        currentPath = pathfinding.FindPath(
            transform.position,
            targetPos
        );

        pathIndex = 0;

        if (currentPath == null)
        {
            Debug.LogWarning("[CustomerNPC] 경로를 찾을 수 없습니다.");
            return;
        }

        if (currentPath.Count == 0)
        {
            // 현재 위치와 목적지가 동일한 그리드에 있는 경우
            moveCoroutine = StartCoroutine(MoveToFinalTarget());
            return;
        }

        moveCoroutine = StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (pathIndex < currentPath.Count)
        {
            // 1. 현재 이동해야 할 노드의 좌표를 가져옴
            Vector3 targetNodePos = currentPath[pathIndex].worldPos;

            // 마지막 노드라면? 격자 중심이 아니라 실좌표로 목적지 교체
            if (pathIndex == currentPath.Count - 1)
            {
                targetNodePos = finalTargetPos;
            }

            targetNodePos.z = transform.position.z;

            // 2. 방향 전환 로직
            if (targetNodePos.x > transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (targetNodePos.x < transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            // 3. 실제 이동
            while (Vector3.Distance(transform.position, targetNodePos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetNodePos,
                    speed * Time.deltaTime
                );
                yield return null;
            }

            pathIndex++;
        }
        OnReachedTarget();
    }

    public void UpdateQueueTarget(Vector3 newTarget)
    {
        // 줄이 줄어들 때마다 호출되어 경로를 갱신합니다.
        SetNewPath(newTarget);
    }

    void OnReachedTarget()
    {
        if (state == ENPCState.MoveToStand)
        {
            state = ENPCState.Pick;
            waitCo = StartCoroutine(CoWaitThenPick());
        }
        else if (state == ENPCState.MoveToCheckout)
        {
            state = ENPCState.Pay;
            waitCo = StartCoroutine(CoWaitThenPay());
        }
        else if (state == ENPCState.Exit)
        {
            Destroy(gameObject);
        }

        RefreshAnim();
    }

    // 앞에 있던 손님이 나가면 뒤에 있던 손님이 앞으로 이동
    public void UpdateStandQueueTarget(Vector3 newTarget)
    {
        // 기존 대기 코루틴 종료
        if (waitCo != null)
        {
            StopCoroutine(waitCo);
            waitCo = null;
        }

        // 다시 매대 이동 상태로 변경
        state = ENPCState.MoveToStand;

        SetNewPath(newTarget);

        RefreshAnim();
    }

    private IEnumerator MoveToFinalTarget()
    {
        Vector3 targetPos = finalTargetPos;
        targetPos.z = transform.position.z;

        yield return null;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
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

}
