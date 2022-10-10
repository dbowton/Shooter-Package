using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Text textField;

    public void display(ref Dictionary<System.Type, List<Item>> inventory)
    {
        StringBuilder sb = new StringBuilder();

        List<System.Type> types = inventory.Keys.ToList();

        foreach(var key in types)
        {
            sb.Append(key.Name);
            sb.Append("\n");

            inventory[key].ForEach(x =>
            {
                sb.Append(x.name);
                sb.Append("\n");
            });
        }

        textField.text = sb.ToString();
    }
}

