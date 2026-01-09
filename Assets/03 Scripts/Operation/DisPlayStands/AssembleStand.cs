using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssembleStand : MonoBehaviour
{
   [Header("스텐드 표출")]
   [SerializeField] 
   private StandView view;

   [Header("레시피")] 
   [SerializeField] 
   private RecipeSO recipe;

   private bool isProcessing;
   private string outputItemId;

    public void Interact(Inventory inv)
    {
       if (recipe == null || inv == null) return;
       
       Debug.Log($"[SashimiStand] Interact! isProcessing={isProcessing}, outputItemId={outputItemId}");
   
       // 1) 출력 있으면 자동 회수
       if (!string.IsNullOrEmpty(outputItemId) )
       {
          Debug.Log($"[SashimiStand] Try pickup: {outputItemId} x{recipe.outputCount}, have={inv.CountOf(outputItemId)}");
          bool ok = inv.TryAdd(outputItemId, recipe.outputCount);
          Debug.Log($"[SashimiStand] pickup result ok={ok}, now={inv.CountOf(outputItemId)}");

          if (!ok)
          {
             Debug.Log("인벤이 가득 차서 회수 불가");
             return;
          }
   
          outputItemId = null;
          view?.ShowOutput(null);
          Debug.Log("[SashimiStand] pickup success -> cleared output");
          return;
       }
   
       // 2) 가공중이면 무시
       if (isProcessing)
       {
          Debug.Log("[SashimiStand] still processing...");
          return;
       }
   
       int haveInput = inv.CountOf(recipe.inputItemId);
       Debug.Log($"[SashimiStand] Try insert: {recipe.inputItemId} x{recipe.inputCount}, have={haveInput}");
       // 3) 재료 자동 투입(인벤에서 차감)
       if (haveInput < recipe.inputCount)
       {
          Debug.Log("재료 부족");
          return;
       }
   
       bool removed = inv.TryRemove(recipe.inputItemId, recipe.inputCount);
       Debug.Log($"[SashimiStand] remove result removed={removed}, left={inv.CountOf(recipe.inputItemId)}");
       if (!removed) return;
   
       StartCoroutine(CoProcess());
    }
   
    private IEnumerator CoProcess()
    {
       isProcessing = true;
       Debug.Log($"[SashimiStand] start process {recipe.processTime}s");
       yield return new WaitForSeconds(recipe.processTime);
   
       outputItemId = recipe.outputItemId;
       view?.ShowOutput(outputItemId);
       Debug.Log($"[SashimiStand] process done -> output={outputItemId}");
   
       isProcessing = false;
    }
}
