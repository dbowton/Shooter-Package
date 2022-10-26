using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleItem
{
    public GameObject visualPrefab;
    public string Name;
    public float Value;

    public string getDesc()
    {
        return ("Name: \t" + Name + "\nValue: \t" + Value);
    }
}
