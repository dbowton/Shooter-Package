using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Container : Interactable
{
    //    private List<Item> itemList = new List<Item>();

    private Dictionary<System.Type, List<Item>> inventory = new Dictionary<System.Type, List<Item>>();

    public List<Item> ItemList {
        get
        {
            List<Item> tempList = new List<Item>();

            foreach (var k in inventory.Keys)
                foreach (var i in inventory[k])
                    tempList.Add(i);
            return tempList;
        }
                
        set 
        {
            inventory.Clear();
            foreach (var i in value) 
            {  
                if(inventory.ContainsKey(i.GetBaseType()))
                {
                    inventory[i.GetBaseType()].Add(i);
                }
                else
                {
                    inventory.Add(i.GetBaseType(), new List<Item>() { i });
                }
            } 
        } 
    }

    [SerializeField] int showItemCount = 3;

    [SerializeField] List<Item> startItems = new List<Item>(0);

    private void Start()
    {
        ItemList = startItems;
    }

    private int index = 0;

    private void OnValidate()
    {
        maxInteractableRange = Mathf.Max(maxInteractableRange, interactableRange);
    }

    public override void Collect()
    {
        Item tempItem = ItemList[index];
        ItemList.Remove(ItemList[index]);

        if (index > 0) index--;
    }

    public override void Remove()
    {
//        itemList.Add(item);
    }

    public Item GetSelectedItem()
    {
        return ItemList[index];
    }

    public override void GenerateText(bool focussed)
    {

        string text;

        text = "";
        text += gameObject.name + "\n";

        if(focussed)
        {
            int start = Mathf.Min(index, Mathf.Max(ItemList.Count - showItemCount, 0));
            int valid = (index > 0 && index < ItemList.Count - 2) ? start - 1 : start;
            for (int i = 0; i < showItemCount; i++, valid++)
            {
                if(valid < start + showItemCount - ((index > 0 && index < ItemList.Count - 2) ? 1 : 0) && valid < ItemList.Count)
                {
                    if (valid == index)
                        text += "- " + ItemList[valid].Name + " -";
                    else
                        text += ItemList[valid].Name;
                }


                text += "\n";
            }

            if (ItemList.Count > 0)
                text += "E - Pickup | ";
            text += "R - Transfer";


            //  Hot-Equip set-up
            /*            if(itemList.Count > 0)
                        {
                            text.text += "E - Pickup";

                            System.Type type = itemList[index].GetBaseType();
                            switch (type)
                            {
                                case System.Type weapon when weapon == typeof(Weapon):
                                case System.Type armour when armour == typeof(Armour):
                                    text.text += " | R - Equip";
                                    break;
                                case System.Type consumable when consumable == typeof(Consumable):
                                    text.text += " | R - Use";
                                    break;
                                default:
                                    throw new System.InvalidOperationException();
                            }
                        }*/

        }
    }

    public void ScrollUp()
    {
        if (index < ItemList.Count - 1) index++;
    }

    public void ScrollDown()
    {
        if (index > 0) index--;
    }
}
