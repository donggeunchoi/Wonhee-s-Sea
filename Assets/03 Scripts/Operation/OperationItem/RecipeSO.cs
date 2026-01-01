using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MachineType
{
    SashimiMaker,
    RiceCooker,
    Assembler,
}



[CreateAssetMenu(fileName = "Recipe_", menuName = "GameData/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("매대의 ID")]
    public string id;

    [Header("어디 사용하는 건지")]
    public MachineType machineType;

    [Header("투입")]
    public List<Ingredient> inputs = new List<Ingredient>();

    [Header("산출")]
    public ItemSO outputItem;
    public int outputCount = 1;

    [Header("과정 시간")]
    public float craftTime = 1f;

}
