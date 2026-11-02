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

    private ENPCState state;
    private Transform currentTarget;

    private Coroutine waitCo;

    // Start is called before the first frame update
    void Start()
    {
        if(anim == null)
            anim = GetComponent<Animator>();

        state = ENPCState.MoveToStand;

        if (targetStand != null)
            currentTarget = targetStand.transform;

        RefreshAnim();
    }
    void RefreshAnim()
    {
        if (anim == null) return;

        bool isWait = (state == ENPCState.Pick || state == ENPCState.Pay);

        bool isMove = (state == ENPCState.MoveToStand || state == ENPCState.MoveToCheckout||state == ENPCState.Exit);

        anim.SetBool(HashIsWait, isWait);
        anim.SetBool(HashIsMove, isMove);
    }
    // Update is called once per frame
    void Update()
    {
      switch(state)
        {
            case ENPCState.MoveToStand:
            case ENPCState.MoveToCheckout:
            case ENPCState.Exit:
                MoveTo();
                break;
        }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (waitCo != null) return;

        // 1) 매대 트리거 들어옴 → Pick
        if (state == ENPCState.MoveToStand && targetStand != null && other.transform == colStorage.transform)
        {
            state = ENPCState.Pick;
            RefreshAnim();
            waitCo = StartCoroutine(CoWaitThenPick());
            return;
        }

        // 2) 계산대 트리거 들어옴 → Pay
        if (state == ENPCState.MoveToCheckout && checkout != null && other.transform == colChechOut.transform)
        {
            state = ENPCState.Pay;
            RefreshAnim();
            waitCo = StartCoroutine(CoWaitThenPay());
            return;
        }
    }

    void TryPickOne()
    {
        // 재고 없으면 Exit
        if (targetStand == null || !targetStand.HasStock(1))
        {
            GoExit();
            return;
        }

        // 꺼내기 실패하면 Exit
        if (!targetStand.TryTake(1))
        {
            GoExit();
            return;
        }

        // 성공하면 인벤 추가
        string itemId = targetStand.SellItemId;
        npcInventory?.TryAdd(itemId, 1);

        // 계산대로 이동
        state = ENPCState.MoveToCheckout;
        currentTarget = checkout != null ? checkout.transform : null;

        // checkout 없으면 그냥 나가게 처리
        if (currentTarget == null)
            GoExit();

        RefreshAnim();
    }

    private void GoExit()
    {
        state = ENPCState.Exit;
        currentTarget = exitPoint;

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

        GoExit();
        RefreshAnim();
    }

    private IEnumerator CoWaitThenPick()
    {
        // waitTime 동안 대기 애니 유지
        yield return new WaitForSeconds(waitTime);

        // 코루틴 끝났으니 비워주기
        waitCo = null;

        // 아직도 Pick 상태일 때만 실행(중간에 다른 이유로 state 변경될 수 있음)
        if (state == ENPCState.Pick)
            TryPickOne();
    }

    private IEnumerator CoWaitThenPay()
    {
        yield return new WaitForSeconds(waitTime);

        waitCo = null;

        if (state == ENPCState.Pay)
            DoPay();
    }


    // 외부에서 세팅용
    public void Init(StorageStand stand, CheckoutStand checkoutStand, Transform exit, Collider2D colstnad, Collider2D colCheck)
    {
        targetStand = stand;
        checkout = checkoutStand;
        exitPoint = exit;
        colStorage = colstnad;
        colChechOut = colCheck;

        state = ENPCState.MoveToStand;
        currentTarget = targetStand != null ? targetStand.transform : null;
        RefreshAnim();
    }

}
