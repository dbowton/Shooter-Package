using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    [SerializeField] public List<SimpleItem> items = new List<SimpleItem>();
    public float money = 0f;
}
