using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiStand : MonoBehaviour
{
   [Header("RecipeSO 데이터")]
   [SerializeField]
   RecipeSO recipeSO;

   public StandManager.StandType Type => StandManager.StandType.Sashimi;
   
   public void Interact(StandManager.StandContext ctx)
   {
      
   }
}
