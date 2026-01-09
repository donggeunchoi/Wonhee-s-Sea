using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiStand : MonoBehaviour
{
   SashimiData sashimiData;

   [SerializeField] private Sprite imgStand;
   
   [Header("스텐드 표출")]
   [SerializeField] 
   private StandView view;

   private bool isProcessing;
   private string outputItemId;

   public void FirstInit(SashimiData _sashimiData)
   {
      sashimiData = _sashimiData;

      //imgStand = JsonManager.Ins.GetJson<>();


   }

   // public void Interact(Inventory inv)
   // {
   //    if (recipeSO == null || inv == null) return;
   //
   //    // 1) 출력 있으면 자동 회수
   //    if (!string.IsNullOrEmpty(outputItemId))
   //    {
   //       bool ok = inv.TryAdd(outputItemId, recipeSO.outputCount);
   //       if (!ok)
   //       {
   //          Debug.Log("인벤이 가득 차서 회수 불가");
   //          return;
   //       }
   //
   //       outputItemId = null;
   //       view?.ShowOutput(null);
   //       return;
   //    }
   //
   //    // 2) 가공중이면 무시
   //    if (isProcessing) return;
   //
   //    // 3) 재료 자동 투입(인벤에서 차감)
   //    if (inv.CountOf(recipeSO.inputItemId) < recipeSO.inputCount)
   //    {
   //       Debug.Log("재료 부족");
   //       return;
   //    }
   //
   //    bool removed = inv.TryRemove(recipeSO.inputItemId, recipeSO.inputCount);
   //    if (!removed) return;
   //
   //    StartCoroutine(CoProcess());
   // }
   //
   // private IEnumerator CoProcess()
   // {
   //    isProcessing = true;
   //    yield return new WaitForSeconds(recipeSO.processTime);
   //
   //    outputItemId = recipeSO.outputItemId;
   //    view?.ShowOutput(outputItemId);
   //
   //    isProcessing = false;
   // }
}

