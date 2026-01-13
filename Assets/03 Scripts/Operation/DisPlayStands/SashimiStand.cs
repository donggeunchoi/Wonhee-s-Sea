using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiStand : MonoBehaviour
{
   SashimiData sashimiData;
   
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
          {
             return;
          }
   
          outputItemId = null;
          view?.ShowOutput(null);
          return;
       }
   
       // 2) 가공중이면 무시
       if (isProcessing)
       {
          return;
       }
   
       int haveInput = inv.CountOf(recipe.inputItemId);
       // 3) 재료 자동 투입(인벤에서 차감)
       if (haveInput < recipe.inputCount)
       {
          Debug.Log("재료 부족");
          return;
       }
   
       bool removed = inv.TryRemove(recipe.inputItemId, recipe.inputCount);
       if (!removed) return;
   
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

