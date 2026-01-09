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
       
       
       // 1) 출력 있으면 자동 회수
       if (!string.IsNullOrEmpty(outputItemId) )
       {
          bool ok = inv.TryAdd(outputItemId, recipe.outputCount);

          if (!ok)
             return;
           
          outputItemId = null;
          view?.ShowOutput(null);
          return;
       }
   
       // 2) 가공중이면 무시
       if (isProcessing)
          return;
       
       var inputs = recipe.GetInputs();
       for (int i = 0; i < inputs.Count; i++)
       {
          var req = inputs[i];
          
          if (string.IsNullOrEmpty(req.itemId) || req.count <= 0)
             continue;

          int have = inv.CountOf(req.itemId);
          if (have < req.count)
          {
             Debug.Log($"재료부족: {req.itemId}가 {req.count}필요 {have}보유");
             return;
          }
       }

       for (int i = 0; i < inputs.Count; i++)
       {
          var req = inputs[i];
          
          if (string.IsNullOrEmpty(req.itemId) || req.count <= 0)
             continue;
          
          bool removed = inv.TryRemove(recipe.inputItemId, recipe.inputCount);
          if (!removed) return;
       }
   
       StartCoroutine(CoProcess());
    }
   
    private IEnumerator CoProcess()
    {
       isProcessing = true;
       yield return new WaitForSeconds(recipe.processTime);
   
       outputItemId = recipe.outputItemId;
       view?.ShowOutput(outputItemId);
       
       isProcessing = false;
    }
}
