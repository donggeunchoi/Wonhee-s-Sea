using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeStand : MonoBehaviour
{
    //도마,밥솥,조리대,판매대(가능하다면)을 한번에 처리하기 위한 .cs
    [Header("레시피")] 
    [SerializeField] private RecipeSO recipe;
    
    [Header("스텐드 표출(결과물 아이콘)")]
    [SerializeField] private StandView view;

    [Header("옵션")] 
    [SerializeField] private bool autoPickupIfPossible = false;


    private bool isProcessing;
    private string outputItemId;
    private int ouputCount;
    
    private Inventory lastInventory;

    public void Interact(Inventory inv)
    {
        if (recipe == null || inv == null)
            return;

        lastInventory = inv;
        
        //결과물이 있으면 회수
        if (!string.IsNullOrEmpty(outputItemId) && ouputCount > 0)
        {
            bool clear = inv.TryAdd(outputItemId, ouputCount);
            
            //클리어가 아니라면 인벤이 가득 찼다는 의미
            if (!clear)
                return;
            
            ClearOutput();
            return;
        }

        //제작중이라면 패스
        if (isProcessing)
            return;

        var inputs = recipe.GetInputs();
        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if(string.IsNullOrEmpty(req.itemId)||req.count <= 0 )
                continue;
            
            int have = inv.CountOf(req.itemId);
            if (have < req.count)
                return;
        }
    }

    //받았다는 코드
    public void ClearOutput()
    {
        outputItemId = null;
        ouputCount = 0;
        view?.ShowOutput(null);
    }
    
}
