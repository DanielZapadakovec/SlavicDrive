using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cooking/Recipe")]
public class RecipeData : ScriptableObject
{
    public ItemType resultItem;
    public float cookTime = 30f;
    public int targetTemperature = 100;
    public int temperatureTolerance = 10;

    public List<ItemType> requiredIngredients;
}