using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe_", menuName = "GameData/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Serializable]
    public class Input
    {
        public string itemId;
        public int count;
    }
    
    [Header ("다중-조립대용 ")]
    public List<Input> inputs = new List<Input>();
    
    [Header("단일 나머지")]
    public string inputItemId;
    public int inputCount = 1;

    [Header("출력")]
    public string outputItemId;
    public int outputCount = 1;

    [Header("시간")]
    public float processTime = 1.5f;
    
    /// <summary>
    /// inputs가 비어있으면 기존 단일 입력을 inputs처럼 취급
    /// </summary>
    public List<Input> GetInputs()
    {
        if (inputs != null && inputs.Count > 0)
            return inputs;

        // 기존 단일 입력 호환
        return new List<Input>
        {
            new Input { itemId = inputItemId, count = inputCount }
        };
    }
}
