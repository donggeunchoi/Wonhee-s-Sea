using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBStandContent : MonoBehaviour
{
    [Header("스텐드ID")]
    [SerializeField]
    int standID;

    [Header("출력 표시 - 월드 오브젝트 쌓기")]
    [SerializeField] private bool useWorldStack = true;
    [SerializeField] private int worldMaxVisible;

    [Header("출력 보관")]
    [SerializeField] private int maxOutputCapacity;
    [SerializeField] private bool autoPickupIfPossible = false;
    [SerializeField] private bool autoContinueIfHasInput = true;

    [Header("아이템 프리팹 조회용")]
    [SerializeField] private ItemAssetManager itemAssetManager;

    [Header("재료 이미지")]
    [SerializeField]
    Image imgItem;
    [SerializeField]
    Transform worldStackRoot;

    StandData standData = null;
    StandRecipeData standRecipeData = null;
    List<StandRecipeInputData> recipeInputDatas = new List<StandRecipeInputData>();

    private bool isProcessing = false;

    // 입력 버퍼 : itemId -> count
    private Dictionary<string, int> inputBuffer = new Dictionary<string, int>();

    // 출력 적재 정보
    private string outputItemId;
    private int outputStockCount = 0;

    private Inventory lastInventory;

    // 월드 스택용 풀
    private readonly List<Image> pooled = new List<Image>();
    private Sprite cachedPrefab;

    void Awake()
    {
        FirstInit();


        //데이터값 잘 들어가고 있음.
        // Debug.LogWarning($"스텐드 ID : {standData.standId}, 이름 : {standData.name}");
    }

    void FirstInit()
    {
        //데이터 캐싱해놓기.
        var standDatas = JsonManager.Ins.GetJson<StandData>();
        if (standDatas == null)
            return;

        standData = standDatas.Find(info => info.standId == standID);
        if (standData == null)
            return;

        var recipeDatas = JsonManager.Ins.GetJson<StandRecipeData>();
        if (recipeDatas == null)
            return;

        standRecipeData = recipeDatas.Find(info => info.standId == standID);
        if (standRecipeData == null)
            return;

        var inputDatas = JsonManager.Ins.GetJson<StandRecipeInputData>();
        if (inputDatas == null)
            return;

        recipeInputDatas = inputDatas.FindAll(info => info.recipeId == standRecipeData.recipeId);
    }

    public void Interact(Inventory inv)
    {
        if (inv == null)
            return;

        if (standData == null || standRecipeData == null)
        {
            Debug.LogWarning($"[CTStandContent] 초기화가 완료되지 않아 Interact 불가. standID={standID}", this);
            return;
        }

        lastInventory = inv;

        // 1. 완성품이 쌓여있으면 우선 회수
        if (!string.IsNullOrEmpty(outputItemId) && outputStockCount > 0)
        {
            TryPickupOutput(inv);
            return;
        }

        // 2. 인벤토리에서 재료를 버퍼로 이동
        DepositInputFromInventory(inv);

        // 3. 현재 생산 중이면 여기까지
        if (isProcessing)
            return;

        // 4. 1회 제작 가능하면 생산 시작
        if (CanConsumeInputForOneCraft())
        {
            ConsumeInputForOneCraft();
            StartCoroutine(CoProcess());
        }
    }

    private void DepositInputFromInventory(Inventory inv)
    {
        if (recipeInputDatas == null || recipeInputDatas.Count == 0)
            return;

        // 한 번 상호작용당 "1회 제작분"만 채우기
        for (int i = 0; i < recipeInputDatas.Count; i++)
        {
            var req = recipeInputDatas[i];

            if (string.IsNullOrEmpty(req.inputItemId1.ToString()) || req.inputCount <= 0)
                continue;

            int buffered = GetBufferedCount(req.inputItemId1.ToString());
            int need = req.inputCount - buffered;

            if (need <= 0)
                continue;

            int have = inv.CountOf(req.inputItemId1.ToString());
            if (have <= 0)
                continue;

            int toMove = Mathf.Min(have, need);

            if (inv.TryRemove(req.inputItemId1.ToString(), toMove))
                AddToBuffer(req.inputItemId1.ToString(), toMove);
        }
    }

    private void AddToBuffer(string itemId, int add)
    {
        if (string.IsNullOrEmpty(itemId) || add <= 0)
            return;

        if (inputBuffer.ContainsKey(itemId))
            inputBuffer[itemId] += add;
        else
            inputBuffer[itemId] = add;
    }

    private int GetBufferedCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return 0;

        if (inputBuffer.TryGetValue(itemId, out int value))
            return value;

        return 0;
    }

    private bool CanConsumeInputForOneCraft()
    {
        if (standRecipeData == null)
            return false;

        if (recipeInputDatas == null || recipeInputDatas.Count == 0)
            return false;

        if (string.IsNullOrEmpty(standRecipeData.outputItemId.ToString()))
            return false;

        if (outputStockCount >= maxOutputCapacity)
            return false;

        for (int i = 0; i < recipeInputDatas.Count; i++)
        {
            var req = recipeInputDatas[i];

            if (string.IsNullOrEmpty(req.inputItemId1.ToString()) || req.inputCount <= 0)
                continue;

            int buffered = GetBufferedCount(req.inputItemId1.ToString());
            if (buffered < req.inputCount)
                return false;
        }

        return true;
    }

    private void ConsumeInputForOneCraft()
    {
        if (recipeInputDatas == null || recipeInputDatas.Count == 0)
            return;

        for (int i = 0; i < recipeInputDatas.Count; i++)
        {
            var req = recipeInputDatas[i];

            if (string.IsNullOrEmpty(req.inputItemId1.ToString()) || req.inputCount <= 0)
                continue;

            inputBuffer[req.inputItemId1.ToString()] -= req.inputCount;

            if (inputBuffer[req.inputItemId1.ToString()] <= 0)
                inputBuffer.Remove(req.inputItemId1.ToString());
        }
    }

    private IEnumerator CoProcess()
    {
        isProcessing = true;

        float processTime = Mathf.Max(0f, standRecipeData.ProcessTime);
        yield return new WaitForSeconds(processTime);

        outputItemId = standRecipeData.outputItemId.ToString();

        if (!string.IsNullOrEmpty(outputItemId) && standRecipeData.outputCount > 0)
        {
            int canStore = Mathf.Max(0, maxOutputCapacity - outputStockCount);
            int add = Mathf.Min(canStore, standRecipeData.outputCount);
            outputStockCount += add;
        }

        isProcessing = false;

        if (autoPickupIfPossible && lastInventory != null && !string.IsNullOrEmpty(outputItemId) && outputStockCount > 0)
            TryPickupOutput(lastInventory);

        RefreshOutputVisual();

        if (autoContinueIfHasInput && !isProcessing && CanConsumeInputForOneCraft())
        {
            ConsumeInputForOneCraft();
            StartCoroutine(CoProcess());
        }
    }

    private void TryPickupOutput(Inventory inv)
    {
        if (inv == null)
            return;

        if (string.IsNullOrEmpty(outputItemId) || outputStockCount <= 0)
            return;

        while (outputStockCount > 0)
        {
            bool ok = inv.TryAdd(outputItemId, 1);
            if (!ok)
                break;

            outputStockCount--;
        }

        if (outputStockCount < 0)
            outputStockCount = 0;

        RefreshOutputVisual();
    }

    private void RefreshOutputVisual()
    {
        if (!useWorldStack)
            return;

        if (worldStackRoot == null)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (string.IsNullOrEmpty(outputItemId) || outputStockCount <= 0)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (itemAssetManager == null)
        {
            Debug.LogWarning("[CTStandContent] itemAssetManager가 연결되지 않음", this);
            SetPoolActiveCount(0);
            return;
        }

        var prefabsprite = itemAssetManager.GetIcon(outputItemId);
        if (prefabsprite == null)
        {
            Debug.LogWarning($"[CTStandContent] output prefab 없음. itemId={outputItemId}", this);
            SetPoolActiveCount(0);
            return;
        }

        if (cachedPrefab != prefabsprite)
        {
            ClearPool();
            cachedPrefab = prefabsprite;
        }

        int visible = Mathf.Clamp(outputStockCount, 0, worldMaxVisible);

        EnsurePool(prefabsprite, visible);
        SetPoolActiveCount(visible);
    }

    private void EnsurePool(Sprite sprite, int needCount)
    {
        if (pooled.Count >= needCount)
            return;

        int toCreate = needCount - pooled.Count;
        for (int i = 0; i < toCreate; i++)
        {
            Image image = Instantiate(imgItem, worldStackRoot);
            image.sprite = sprite;
            image.gameObject.SetActive(false);

            pooled.Add(image);
        }
    }

    private void SetPoolActiveCount(int activeCount)
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] == null)
                continue;

            bool active = i < activeCount;
            pooled[i].gameObject.SetActive(active);
        }
    }

    private void ClearPool()
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] != null)
                Destroy(pooled[i]);
        }

        pooled.Clear();
    }

    // 디버그 / 외부 조회용
    public bool IsProcessing => isProcessing;
    public int OutputStockCount => outputStockCount;
    public string OutputItemId => outputItemId;
    public int StandID => standID;   
}
