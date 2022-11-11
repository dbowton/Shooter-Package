using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SimpleItem
{
    public GameObject visualPrefab;
    public Image visualIcon;

    [HideInInspector] public GameObject visualObject;

    public string Name;
    public float Value;
    public ItemId id;

    public enum ItemId
    {
        wood
    }

    public string getDesc()
    {
        return ("Name: \t" + Name + "\nValue: \t" + Value);
    }
}
