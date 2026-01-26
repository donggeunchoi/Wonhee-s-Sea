using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeStand : MonoBehaviour
{
    //도마,밥솥,조리대,판매대(가능하다면)을 한번에 처리하기 위한 .cs
    [Header("레시피")] 
    [SerializeField] private RecipeSO recipe;
    
    [Header("스텐드 표출(결과물 아이콘)")]
    [SerializeField] private StandView view;

    [Header("옵션")] 
    [SerializeField] private bool autoPickupIfPossible = false;

    [Header("스텐드 기본 정보")] 
    [SerializeField] private TextMeshProUGUI StandNametext;
    [SerializeField] private Image imgIcon;

    [Header("쌓일 프리펩")]
    [SerializeField]
    GameObject Prefab;

    
    private bool isProcessing;
    private string outputItemId;
    private int ouputCount;
    
    private Inventory lastInventory;
    private PlayerStackVisualizer stack;

    private void Awake()
    {
        if(stack == null)
            stack = GetComponent<PlayerStackVisualizer>();
    }

    public void Interact(Inventory inv, PlayerStackVisualizer stack)
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

            int needRemove = 0;
            var input = recipe.GetInputs();
            for(int i = 0; i < input.Count; i++)
            {
                needRemove += input[i].count;
            }



            if (stack != null && Prefab != null)
                for(int i = 0; i < ouputCount; i++)
                    stack.Add(Prefab);

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

        for (int i = 0; i < inputs.Count; i++)
        {
            var req = inputs[i];
            if(string.IsNullOrEmpty(req.itemId)||req.count <= 0)
                continue;
            
            bool remove = inv.TryRemove(req.itemId, req.count);
            if (!remove)
            {
                Debug.LogWarning("재료 차감 실패");
                return;
            }
        }

        StartCoroutine(CoProcess());
    }

    private IEnumerator CoProcess()
    {
        isProcessing = true;
        view?.ShowProcessing(true);

        yield return new WaitForSeconds(recipe.processTime);

        outputItemId = recipe.outputItemId;
        ouputCount = recipe.outputCount;

        view?.ShowProcessing(false);
        view?.ShowOutput(outputItemId);
        
        isProcessing = false;

        if (autoPickupIfPossible && lastInventory != null)
        {
            bool ok = lastInventory.TryAdd(outputItemId, ouputCount);
            if (ok)
            {
                if (stack != null && Prefab != null)
                    stack.Add(Prefab);


                ClearOutput();
            }
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
