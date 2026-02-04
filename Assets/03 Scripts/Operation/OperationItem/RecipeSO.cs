using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeInput
{
    public ItemSO item;
    public int count;
}

[CreateAssetMenu(fileName = "Recipe_", menuName = "GameData/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("다중 입력")]
    public List<RecipeInput> inputs = new();

    [Header("기존 단일 입력")]
    [SerializeField] private ItemSO inputItem;
    [SerializeField] private int inputCount = 1;

    [Header("출력 결과")]
    public ItemSO outputItem;
    public int outputCount = 1;

    [Header("제작 시간")]
    public float processTime = 1.5f;

    /// <summary>
    /// 다중 입력이 있으면 inputs를 사용하고,
    /// 비어있으면 기존 단일 입력을 1개짜리 리스트로 반환
    /// </summary>
    public List<RecipeInput> GetInputs()
    {
        // 다중 입력 우선
        if (inputs != null && inputs.Count > 0)
            return inputs;

        // 기존 단일 입력 호환
        if (inputItem != null && inputCount > 0)
        {
            return new List<RecipeInput>
            {
                new RecipeInput { item = inputItem, count = inputCount }
            };
        }

        // 아무것도 없으면 빈 리스트
        return new List<RecipeInput>();
    }
}