using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeStand : MonoBehaviour
{
    [Header("레시피")]
    [SerializeField] private RecipeSO recipe;

    [Header("아이템 DB (월드 오브젝트 스폰용)")]
    [SerializeField] private ItemDatabaseSO itemDatabase;

    [Header("옵션")]
    [SerializeField] private bool autoPickupIfPossible = false;

    [Header("출력 표시 - A(월드 오브젝트 쌓기)")]
    [SerializeField] private bool useWorldStack;
    [SerializeField] private Transform worldStackRoot;
    [SerializeField] private int columnCount;
    [SerializeField] private float worldStackOffsetX;
    [SerializeField] private float worldStackOffsetY;
    [SerializeField] private int worldMaxVisible; // 실제로 쌓아두는 오브젝트는 제한(성능)

    [Header("스탠드 저장공간")]
    [SerializeField] private int maxOutputCapacity = 30; // 출력 최대 보관
    [SerializeField] private bool autoContinueIfHasInput = true; // 입력이 남아있으면 연속 생산

    private bool isProcessing;
    private Dictionary<string, int> inputBuffer = new Dictionary<string, int>();
    private ItemSO outputItem;
    private int outputStockCount;
    private Inventory lastInventory;
    private readonly List<GameObject> worldSpawned = new List<GameObject>();

    private List<GameObject> pooled = new List<GameObject>();
    private GameObject cachedPrefab;//현재 풀링이 어떤 프리펩 기준인지 확인하기위해

    public void Interact(Inventory inv)
    {
        if (recipe == null || inv == null) return;
        lastInventory = inv;

        // 1) 출력이 쌓여있으면 -> 회수 먼저
        if (outputItem != null && outputStockCount > 0)
        {
            TryPickupOutput(inv);
            return;
        }

        // 2) 생산 중이면 추가 투입만(선택)
        //    (체인소주스킹 느낌이면 생산 중에도 투입 가능하게 해도 됨)
        //    여기서는 "생산 중에는 투입만 받고 시작은 생산 종료 후"로 처리
        DepositInputFromInventory(inv);

        // 3) 생산 시작 조건
        if (isProcessing) return;

        if (CanConsumeInputForOneCraft())
        {
            ConsumeInputForOneCraft();
            StartCoroutine(CoProcess());
        }
    }

    private void DepositInputFromInventory(Inventory inv)
    {
        var inputs = recipe.GetInputs();
        if (inputs == null || inputs.Count == 0) return;

        // 한 번 상호작용당 "1회 제작분"만큼만 투입(자동 상호작용이 반복되며 여러 번 투입됨)
        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if (req.item == null || req.count <= 0) continue;

            string id = req.item.itemId;

            int buffered = GetBufferedCount(id);
            int need = req.count - buffered;
            if (need <= 0) continue;

            int have = inv.CountOf(id);
            if (have <= 0) continue;

            int toMove = Mathf.Min(have, need);

            if (inv.TryRemove(id, toMove))
                AddToBuffer(id, toMove);
        }
    }

    private void AddToBuffer(string itemId, int add)
    {
        if (add <= 0) return;
        if (inputBuffer.ContainsKey(itemId)) inputBuffer[itemId] += add;
        else inputBuffer[itemId] = add;
    }

    private int GetBufferedCount(string itemId)
    {
        if (inputBuffer.TryGetValue(itemId, out int v)) return v;
        return 0;
    }

    private bool CanConsumeInputForOneCraft()
    {
        var inputs = recipe.GetInputs();
        if (inputs == null || inputs.Count == 0) return false;

        // 출력 용량 체크
        if (outputItem == null) outputItem = recipe.outputItem;
        if (outputItem == null) return false;

        if (outputStockCount >= maxOutputCapacity) return false;

        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if (req.item == null || req.count <= 0) continue;

            int buffered = GetBufferedCount(req.item.itemId);
            if (buffered < req.count) return false;
        }
        return true;
    }

    private void ConsumeInputForOneCraft()
    {
        var inputs = recipe.GetInputs();
        if (inputs == null || inputs.Count == 0) return;

        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if (req.item == null || req.count <= 0) continue;

            string id = req.item.itemId;
            inputBuffer[id] -= req.count;
            if (inputBuffer[id] <= 0) inputBuffer.Remove(id);
        }
    }

    private IEnumerator CoProcess()
    {
        isProcessing = true;

        // 생산 중 표시(필요하면)
        // storageItem?.ClearAll();

        yield return new WaitForSeconds(recipe.processTime);

        outputItem = recipe.outputItem;
        if (outputItem != null && recipe.outputCount > 0)
        {
            int canStore = Mathf.Max(0, maxOutputCapacity - outputStockCount);
            int add = Mathf.Min(canStore, recipe.outputCount);
            outputStockCount += add;
        }

        isProcessing = false;

        // 자동 픽업 옵션(원하면 유지)
        if (autoPickupIfPossible && lastInventory != null && outputItem != null && outputStockCount > 0)
        {
            // 한 번에 다 못 넣을 수도 있으니 "가능한 만큼" 넣고 남기기
            TryPickupOutput(lastInventory);
        }

        RefreshOutputVisual();

        // 입력이 버퍼에 남아있고, 출력 공간도 있고, 연속 생산 켜져 있으면 다시 제작
        if (autoContinueIfHasInput && !isProcessing && CanConsumeInputForOneCraft())
        {
            ConsumeInputForOneCraft();
            StartCoroutine(CoProcess());
        }
    }

    private void TryPickupOutput(Inventory inv)
    {
        if (inv == null) return;
        if (outputItem == null || outputStockCount <= 0) return;

        // Inventory.TryAdd가 "전부 들어가야 true" 구조라서,
        // partial 지원이 없다면 1개씩 넣는 방식으로 처리(안전)
        int moved = 0;
        while (outputStockCount > 0)
        {
            bool ok = inv.TryAdd(outputItem.itemId, 1);
            if (!ok) break;

            outputStockCount--;
            moved++;
        }

        if (outputStockCount <= 0)
        {
            outputStockCount = 0;
            // outputItem은 유지해도 되지만 명확하게 지우고 싶으면 아래 주석 해제
            // outputItem = null;
        }

        RefreshOutputVisual();
    }

    private void RefreshOutputVisual()
    {
        // A) 월드 오브젝트 표시
        if (!useWorldStack) 
            return;

        if (worldStackRoot == null || itemDatabase == null || outputItem == null || outputStockCount <= 0)
        {
            SetPoolActiveCount(0);
            return;
        }

        var prefab = outputItem.prefab;
        if (prefab == null)
        {
            SetPoolActiveCount(0);
            return;
        }

        if (cachedPrefab != prefab)
        {
            ClearPool();
            cachedPrefab = prefab;
        }

        int visible = Mathf.Clamp(outputStockCount, 0, worldMaxVisible);

        EnsurePool(prefab, visible);
        SetPoolActiveCount(visible);
        LayoutPoolGrid(visible);
        
    }

    private void EnsurePool(GameObject prefab, int needCount)
    {
        if (pooled.Count >= needCount) return;

        int toCreate = needCount - pooled.Count;
        for (int i = 0; i < toCreate; i++)
        {
            var go = Instantiate(prefab, worldStackRoot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);
            pooled.Add(go);
        }
    }

    private void SetPoolActiveCount(int activeCount)
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            bool active = i < activeCount;
            if (pooled[i] != null && pooled[i].activeSelf != active)
                pooled[i].SetActive(active);
        }
    }

    private void LayoutPoolGrid(int visibleCount)
    {
        // 안전장치
        if (columnCount <= 0) columnCount = 5;

        for (int i = 0; i < visibleCount; i++)
        {
            var go = pooled[i];
            if (go == null) continue;

            int col = i % columnCount;  // 0~(columnCount-1)
            int row = i / columnCount;  // 0,1,2...

            go.transform.localPosition = new Vector3(col * worldStackOffsetX, row * worldStackOffsetY, 0f);
            go.transform.localRotation = Quaternion.identity;
        }
    }

    private void ClearPool()
    {
        for (int i = 0; i < pooled.Count; i++)
        {
            if (pooled[i] != null) Destroy(pooled[i]);
        }
        pooled.Clear();
    }
}