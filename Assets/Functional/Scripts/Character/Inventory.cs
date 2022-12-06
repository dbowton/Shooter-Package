using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Inventory : MonoBehaviour
{
    [HideInInspector] public bool isPlayer = false;
    public Weapon activeWeapon;
    public List<Weapon> weapons;

    private List<Item> items = new List<Item>();

    public List<Item> Items { get { return items; } set { items = value; } }

    public List<Item> startingItems = new List<Item>();
    public float money = 0f;

    public List<SimpleItem> simpleItems = new List<SimpleItem>();

    private void Start()
    {
        items.Clear();
        items.AddRange(startingItems);
    }

    public void AddItem(Item item)
    {
        item.transform.parent = gameObject.transform;

        if (isPlayer && item is Weapon)
        {
            weapons.Add(item as Weapon);
        }
        else items.Add(item);
    }
}*/
