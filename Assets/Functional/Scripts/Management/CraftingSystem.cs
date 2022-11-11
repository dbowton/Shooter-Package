using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SimpleItem;

public class CraftingSystem : Singleton<CraftingSystem>
{
    [System.Serializable]
    public class CraftingMaterial
    {
        public SimpleItem item;
        public int count;
    }

    [System.Serializable]
    public class CraftingRecipe
    {
        public List<CraftingMaterial> requiredMaterials = new List<CraftingMaterial>();
        public CraftingMaterial outputMaterial;
    }

    public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

    public bool CraftItem(Inventory inventory, ItemId desiredItem)
    {
        CraftingRecipe recipe = craftingRecipes.Find(x => x.outputMaterial.item.Equals(desiredItem));

        List<SimpleItem> usedItems = new List<SimpleItem>();

        foreach (var material in recipe.requiredMaterials)
        {
            if (inventory.simpleItems.Where(x => x.id.Equals(material.item.id)).Count() < material.count) return false;
            else usedItems.AddRange(inventory.simpleItems.Where(x => x.id.Equals(material.item.id)).ToList().GetRange(0, material.count));
        }

        foreach (var usedItem in usedItems)
        {
            inventory.simpleItems.Remove(usedItem);
        }

        for (int i = 0; i < recipe.outputMaterial.count; i++)
            inventory.simpleItems.Add(recipe.outputMaterial.item);

        return true;
    }
}
