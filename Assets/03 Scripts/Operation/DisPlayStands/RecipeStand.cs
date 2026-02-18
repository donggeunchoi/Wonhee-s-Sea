using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeStand : MonoBehaviour
{
    [Header("레시피")]
    [SerializeField] private RecipeSO recipe;

    [Header("옵션")]
    [SerializeField] private bool autoPickupIfPossible = false;

    [Header("출력 선반(아이콘 표시)")]
    [SerializeField]
    StorageItem storageItem;

    private bool isProcessing;

    private ItemSO outputItem;
    private int outputCount;

    private Inventory lastInventory;

    public void Interact(Inventory inv)
    {
        if (recipe == null || inv == null)
            return;

        lastInventory = inv;

        if (outputItem != null && outputCount > 0)
        {
            bool ok = inv.TryAdd(outputItem.itemId, outputCount);
            if (!ok) return; 

            ClearOutput();
            return;
        }

        if (isProcessing) return;

        List<RecipeInput> inputs = recipe.GetInputs();
        if (inputs == null || inputs.Count == 0) return;

        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if (req.item == null || req.count <= 0) continue;

            int have = inv.CountOf(req.item.itemId);
            if (have < req.count) return;
        }

        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if (req.item == null || req.count <= 0) continue;

            bool removed = inv.TryRemove(req.item.itemId, req.count);
            if (!removed)
            {
                Debug.LogWarning("RecipeStand: 재료 차감 실패");
                return;
            }
        }

        StartCoroutine(CoProcess());
    }

    private IEnumerator CoProcess()
    {
        isProcessing = true;
        storageItem?.ClearAll();

        yield return new WaitForSeconds(recipe.processTime);

        outputItem = recipe.outputItem;
        outputCount = recipe.outputCount;
        
        isProcessing = false;

        if (autoPickupIfPossible && lastInventory != null && outputItem != null && outputCount > 0)
        {
            bool ok = lastInventory.TryAdd(outputItem.itemId, outputCount);
            if (ok)
            {
                ClearOutput();
                yield break;
                
            }
        }

        if (storageItem != null && outputItem != null && outputCount > 0)
        {
            // StorageItem 쪽에서 itemId에 맞는 아이콘 스프라이트를 표시하도록
            storageItem.Show(outputItem, outputCount);
        }

    }

    private void ClearOutput()
    {
        outputItem = null;
        outputCount = 0;

        if (storageItem != null)
            storageItem.ClearAll();
    }
}