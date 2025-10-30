using UnityEngine;
[System.Serializable]
public class IngredientData
{
    [Header("Cooking properties")]
    public float volume = 1f; // v litroch
    [Range(0, 1)] public float sweetness = 0.5f;
    [Range(0, 1)] public float acidity = 0.1f;
    [Range(0, 1)] public float potentialAlcohol = 0.08f; // 8% (0.08)
    public bool isFermentable = true;
}