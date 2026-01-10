using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    [Header("Recipes")]
    public List<RecipeData> recipes;

    [Header("References")]
    public InventoryQuickBar inventory;
    public StorageObject outputStorage;
    public GameObject player;

    [Header("MiniGames")]
    public WaterFill waterMiniGame;
    public TemperatureMiniGame temperatureMiniGame;

    [Header("UI")]
    public GameObject waterUI;
    public GameObject cookingUI;

    public List<ItemType> currentIngredients = new List<ItemType>();
    private RecipeData activeRecipe;

    private CookingState state = CookingState.Idle;

    [Header("Animations")]
    public Animator kitchenAnimator;


    [Header("CookingCamera")]
    public Camera cookingCamera;


    // =========================
    // INTERACT
    // =========================
    public void Interact()
    {
        if (state != CookingState.Idle)
            return;

        ItemType heldItem = inventory.slots[inventory.activeSlot].itemType;

        if (heldItem != ItemType.None)
        {
            TryAddIngredient(heldItem);
            return;
        }

        if (!CanCook())
        {
            UIManager.Instance.ShowErrorMessage("Missing or wrong ingredients");
            return;
        }

        if (outputStorage.CountItem(ItemType.Jar) <= 0)
        {
            UIManager.Instance.ShowErrorMessage("You need empty jars to cook!");
            return;
        }

        StartWaterPhase();
    }
    // =========================
    // INGREDIENTS
    // =========================
    private void TryAddIngredient(ItemType type)
    {
        if (!IsIngredientValid(type))
            return;

        currentIngredients.Add(type);
        inventory.ClearSlot(inventory.activeSlot);

        Debug.Log($"Ingredient added: {type}");
    }

    private bool IsIngredientValid(ItemType type)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.requiredIngredients.Contains(type))
                return true;
        }
        return false;
    }

    // =========================
    // RECIPE CHECK
    // =========================
    private bool CanCook()
    {
        foreach (var recipe in recipes)
        {
            if (MatchesRecipe(recipe))
            {
                activeRecipe = recipe;
                return true;
            }
        }
        return false;
    }

    private bool MatchesRecipe(RecipeData recipe)
    {
        if (currentIngredients.Count != recipe.requiredIngredients.Count)
            return false;

        foreach (var req in recipe.requiredIngredients)
        {
            if (!currentIngredients.Contains(req))
                return false;
        }
        return true;
    }

    // =========================
    // WATER PHASE
    // =========================
    private void StartWaterPhase()
    {
        state = CookingState.FillingWater;
        PlayerController.SwitchingCameraMovement();
        player.SetActive(false);
        cookingCamera.gameObject.SetActive(true);
        kitchenAnimator.SetBool("isWater", true);

        waterMiniGame.ResetWater();
        waterUI.SetActive(true);
        cookingUI.SetActive(false);

    }

    private void Update()
    {
        if (state == CookingState.FillingWater && waterMiniGame.IsFull)
        {
            StartCookingPhase();
        }

        if (state == CookingState.Cooking && temperatureMiniGame.IsFinished)
        {
            FinishCooking();
        }
    }

    // =========================
    // COOKING PHASE
    // =========================
    private void StartCookingPhase()
    {
        state = CookingState.Cooking;
        kitchenAnimator.SetBool("isWater", false);

        waterUI.SetActive(false);
        cookingUI.SetActive(true);
    }

    // =========================
    // FINISH
    // =========================
    private void FinishCooking()
    {
        bool perfect =
            Mathf.Abs(temperatureMiniGame.temperature - activeRecipe.targetTemperature)
            <= activeRecipe.temperatureTolerance;

        int jarCount = outputStorage.CountItem(activeRecipe.emptyJarItem);

        outputStorage.RemoveItems(activeRecipe.emptyJarItem, jarCount);

        ItemType resultItem = perfect
            ? activeRecipe.perfectResultItem
            : activeRecipe.imperfectResultItem;

        for (int i = 0; i < jarCount; i++)
        {
            outputStorage.storedItems.Add(resultItem);
        }

        Debug.Log(perfect
            ? $"Perfect Jam x{jarCount}"
            : $"Imperfect Jam x{jarCount}");

        PlayerController.SwitchingCameraMovement();
        cookingCamera.gameObject.SetActive(false);
        player.SetActive(true);

        ResetCooking();
    }

    private void ResetCooking()
    {
        currentIngredients.Clear();
        activeRecipe = null;

        waterUI.SetActive(false);
        cookingUI.SetActive(false);

        state = CookingState.Idle;
    }

}
