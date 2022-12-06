using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] List<CraftingSystem.FullAutoRecipe> fullAutoRecipes = new List<CraftingSystem.FullAutoRecipe>();
    [SerializeField] List<CraftingSystem.SemiAutoRecipe> semiAutoRecipes = new List<CraftingSystem.SemiAutoRecipe>();
    [SerializeField] List<CraftingSystem.BurstRecipe> burstRecipes = new List<CraftingSystem.BurstRecipe>();
    [SerializeField] List<CraftingSystem.ChargeRecipe> chargeRecipes = new List<CraftingSystem.ChargeRecipe>();

    public List<CraftingSystem.CraftingRecipe> recipes = new List<CraftingSystem.CraftingRecipe>();

    private void Start()
    {
        foreach(var recipe in fullAutoRecipes)
        {
            recipes.Add(recipe);
        }
        foreach (var recipe in semiAutoRecipes)
        {
            recipes.Add(recipe);
        }
        foreach (var recipe in burstRecipes)
        {
            recipes.Add(recipe);
        }
        foreach (var recipe in chargeRecipes)
        {
            recipes.Add(recipe);
        }
    }

    [SerializeField] GameObject recipePrefab;
    [SerializeField] GameObject managerPrefab;
    UICraftingManager uiCraftingManager;
    List<UICraftingInfo> uiCraftingInfos = new List<UICraftingInfo>();

    public void SetUp(ComplexInventory inventory)
    {
        uiCraftingManager = Instantiate(managerPrefab).GetComponent<UICraftingManager>();

        foreach (var recipe in recipes)
        {
            UICraftingInfo newInfo = Instantiate(recipePrefab, uiCraftingManager.parentObject.transform).GetComponent<UICraftingInfo>();
            newInfo.setUp(recipe);
            uiCraftingInfos.Add(newInfo);
        }
    }

    public void update(ComplexInventory inventory)
    {
        foreach(var info in uiCraftingInfos)
        {
            info.update(inventory);
        }
    }

    private void Update()
    {
        if(uiCraftingManager)
        {
            update(GameManager.Instance.player.playerData.inventory);

            if(Input.GetKeyDown(KeyCode.L))
            {
                GameManager.Instance.EndCrafting();
            }
        }
    }

    public void ShutDown()
    {
        Destroy(uiCraftingManager.gameObject);
        uiCraftingManager = null;
        uiCraftingInfos.Clear();
    }
}
