using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTStandContent : MonoBehaviour
{
    [Header("스텐드ID")]
    [SerializeField]
    int standID;

    StandData standData = null;
    StandRecipeData standRecipeData = null;
    List<StandRecipeInputData> recipeInputDatas = new List<StandRecipeInputData>();

    private bool isProcessing = false;
    private Dictionary<int, int> inputBuffer = new Dictionary<int, int>();
    private int outputStockCount = 0;
    private Inventory lastInventory = null;

    void Awake()
    {
        FirstInit();
       if(standData == null)
        {
            var standDatas = JsonManager.Ins.GetJson<StandData>();

            standData = standDatas.Find(info => info.standId == standID);
        }

       //데이터값 잘 들어가고 있음.
        Debug.LogWarning($"스텐드 ID : {standData.standId}, 이름 : {standData.name}");
    }

    void FirstInit()
    {
        //데이터 캐싱해놓기.
        var standDatas = JsonManager.Ins.GetJson<StandData>();
        standData = standDatas.Find(info => info.standId == standID);

        var recipeDatas = JsonManager.Ins.GetJson<StandRecipeData>();
        standRecipeData = recipeDatas.Find(info => info.standId == standID);

        var inputDatas = JsonManager.Ins.GetJson<StandRecipeInputData>();
        if(recipeDatas != null)
            recipeInputDatas = inputDatas.FindAll(info => info.recipeId == standRecipeData.recipeId);

        if (standData == null)
            return;

        if (recipeDatas == null)
            return;
    }

    public void Interact(Inventory inv)
    {
        if (standData == null || standRecipeData == null || inv == null)
            return;

        lastInventory = inv;

        if (outputStockCount > 0)
        {
            //TryPickupOutput(inv);
            return;
        }

        DepositInputFromInventory(inv);

        if (isProcessing)
            return;

        if (CanConsumeInputForOneCraft())
        {
            ConsumeInputForOneCraft();
            //StartCoroutine(CoProcess());
        }
    }

    private void DepositInputFromInventory(Inventory inv)
    {
        if (recipeInputDatas == null || recipeInputDatas.Count == 0)
            return;

        for (int i = 0; i < recipeInputDatas.Count; i++)
        {
            var req = recipeInputDatas[i];
            if (req.inputCount <= 0)
                continue;

            //int buffered = GetBufferedCount(req.inputItemId1);
            //int need = req.inputCount - buffered;
            //if (need <= 0)
            //    continue;

            //int have = inv.CountOf(req.inputItemId.ToString());
            //if (have <= 0)
            //    continue;

            //int toMove = Mathf.Min(have, need);

            //if (inv.TryRemove(req.inputItemId.ToString(), toMove))
            //    AddToBuffer(req.inputItemId, toMove);
        }
    }

    private void AddToBuffer(int itemId, int add)
    {
        if (add <= 0)
            return;

        if (inputBuffer.ContainsKey(itemId))
            inputBuffer[itemId] += add;
        else
            inputBuffer[itemId] = add;
    }

    private int GetBufferedCount(int itemId)
    {
        if (inputBuffer.TryGetValue(itemId, out int value))
            return value;

        return 0;
    }

    private bool CanConsumeInputForOneCraft()
    {
        //if (recipeData == null)
        //    return false;

        //if (outputStockCount >= maxOutputCapacity)
        //    return false;

        //for (int i = 0; i < recipeInputDatas.Count; i++)
        //{
        //    var req = recipeInputDatas[i];
        //    int buffered = GetBufferedCount(req.inputItemId);

        //    if (buffered < req.inputCount)
        //        return false;
        //}

        return true;
    }

    private void ConsumeInputForOneCraft()
    {
        //for (int i = 0; i < recipeInputDatas.Count; i++)
        //{
        //    var req = recipeInputDatas[i];

        //    inputBuffer[req.inputItemId] -= req.inputCount;

        //    if (inputBuffer[req.inputItemId] <= 0)
        //        inputBuffer.Remove(req.inputItemId);
        //}
    }

    //private IEnumerator CoProcess()
    //{
    //    isProcessing = true;

    //    yield return new WaitForSeconds(recipeData.processTime);

    //    int canStore = Mathf.Max(0, maxOutputCapacity - outputStockCount);
    //    int add = Mathf.Min(canStore, recipeData.outputCount);
    //    outputStockCount += add;

    //    isProcessing = false;

    //    if (autoPickupIfPossible && lastInventory != null && outputStockCount > 0)
    //        TryPickupOutput(lastInventory);

    //    if (autoContinueIfHasInput && !isProcessing && CanConsumeInputForOneCraft())
    //    {
    //        ConsumeInputForOneCraft();
    //        StartCoroutine(CoProcess());
    //    }
    //}

    //private void TryPickupOutput(Inventory inv)
    //{
    //    if (inv == null || recipeData == null || outputStockCount <= 0)
    //        return;

    //    while (outputStockCount > 0)
    //    {
    //        bool ok = inv.TryAdd(recipeData.outputItemId.ToString(), 1);
    //        if (!ok)
    //            break;

    //        outputStockCount--;
    //    }

    //    if (outputStockCount < 0)
    //        outputStockCount = 0;
    //}
}
