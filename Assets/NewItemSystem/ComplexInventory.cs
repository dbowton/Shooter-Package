using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComplexInventory : MonoBehaviour
{
    [HideInInspector] public bool isPlayer = false;
    [SerializeField] List<FullAutoData> fullAutoGuns = new List<FullAutoData>();
    [SerializeField] List<MiscData> miscItems = new List<MiscData>();
    public WeaponData activeWeapon = null;

    public float money = 0;

    public List<WeaponData> Weapons { get 
        {
            List<WeaponData> weapons = new List<WeaponData>();
            foreach (var item in items)
            {
                if (item is WeaponData) weapons.Add(item as WeaponData);
            }

            return weapons;
        } 
    }

    [HideInInspector] public int weaponIndex = 0;

    [HideInInspector] public List<ItemData> items = new List<ItemData>();
    void Start()
    {
        foreach (var weapon in fullAutoGuns)
            items.Add(weapon);

        foreach (var misc in miscItems)
            items.Add(misc);
    }
}
