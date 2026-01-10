using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cooking/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Results")]
    public ItemType perfectResultItem;
    public ItemType imperfectResultItem;

    [Header("Cooking")]
    public float cookTime = 30f;
    public int targetTemperature = 100;
    public int temperatureTolerance = 10;

    [Header("Ingredients")]
    public List<ItemType> requiredIngredients;

    [Header("Containers")]
    public ItemType emptyJarItem = ItemType.Jar;
}
