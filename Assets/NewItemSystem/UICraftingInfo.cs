using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICraftingInfo : MonoBehaviour
{
    public Image outputIcon;
    public TMPro.TMP_Text outputName;
    public TMPro.TMP_Text outputDesc;

    public Image resource1Icon;
    public TMPro.TMP_Text resource1Count;
    public Image resource2Icon;
    public TMPro.TMP_Text resource2Count;

    public Button craftButton;

    [HideInInspector] public CraftingSystem.CraftingRecipe recipe;

    public void setUp(CraftingSystem.CraftingRecipe recipe)
    {
        this.recipe = recipe;

        outputIcon.sprite = recipe.GetOutput().itemIcon;
        outputName.text = recipe.GetOutput().Name;
        outputDesc.text = recipe.GetOutput().GetDesc();

        //        resource1Icon.sprite = recipe.requiredMaterials[0].resource;
        resource1Count.text = recipe.requiredMaterials[0].resource.ToString() + recipe.requiredMaterials[0].requiredAmount;
        resource2Count.text = recipe.requiredMaterials[1].resource.ToString() + recipe.requiredMaterials[1].requiredAmount;

        craftButton.onClick.AddListener(() => CraftingSystem.CraftItem(GameManager.Instance.player.playerData.inventory, recipe));
    }

    public void update(ComplexInventory inventory)
    {
        if(CraftingSystem.CanCraftItem(inventory, recipe))
        {
            craftButton.GetComponent<Image>().color = Color.green;
        }
        else craftButton.GetComponent<Image>().color = Color.red;
    }
}
