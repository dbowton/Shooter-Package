using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SimpleItem;

public static class CraftingSystem
{
    [System.Serializable]
    public class CraftingMaterial
    {
        public MiscData.ItemID resource;
        public int requiredAmount;
    }

    [System.Serializable] public abstract class CraftingRecipe 
    {
        public List<CraftingMaterial> requiredMaterials = new List<CraftingMaterial>();
        public abstract ItemData GetOutput();
    }

    #region Gun Recipes
    [System.Serializable]
    public class FullAutoRecipe : CraftingRecipe
    {
        public FullAutoData outputMaterial;

        public override ItemData GetOutput()
        {
            return outputMaterial;
        }
    }
    [System.Serializable]
    public class SemiAutoRecipe : CraftingRecipe
    {
        public SemiAutoData outputMaterial;
        public override ItemData GetOutput()
        {
            return outputMaterial;
        }
    }
    [System.Serializable]
    public class BurstRecipe : CraftingRecipe
    {
        public BurstData outputMaterial;
        public override ItemData GetOutput()
        {
            return outputMaterial;
        }
    }
    [System.Serializable]
    public class ChargeRecipe : CraftingRecipe
    {
        public ChargeData outputMaterial;
        public override ItemData GetOutput()
        {
            return outputMaterial;
        }
    }
    #endregion

    public static bool CraftItem(ComplexInventory inventory, CraftingRecipe recipe)
    {
        List<MiscData> requiredItems = new List<MiscData>();

        if (!CanCraftItem(inventory, recipe)) return false;

        foreach(var item in recipe.requiredMaterials)
        {
            int workingCount = 0;
            while(workingCount < item.requiredAmount)
            {
                ItemData usedItem = inventory.items.First(x => x is MiscData && (x as MiscData).craftingID.Equals(item.resource));
                workingCount += (usedItem as MiscData).craftingCount;
                inventory.items.Remove(usedItem);
            }
        }

        inventory.items.Add(recipe.GetOutput());
        return true;
    }

    public static bool CanCraftItem(ComplexInventory inventory, CraftingRecipe recipe)
    {
        List<MiscData> requiredItems = new List<MiscData>();

        foreach (var item in recipe.requiredMaterials)
        {
            if (inventory.items.Where(x => (x is MiscData) && (x as MiscData).craftingID.Equals(item.resource)).Sum(x => (x as MiscData).craftingCount) < item.requiredAmount) return false;
        }

        return true;
    }
}
