using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe_", menuName = "GameData/Recipe")]
public class RecipeSO : ScriptableObject
{
    public string inputItemId;
    public int inputCount = 1;

    public string outputItemId;
    public int outputCount = 1;

    public float processTime = 1.5f;
}
