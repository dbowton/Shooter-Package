using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] Material material;

    public void ChangeColor(string newColor)
    {
        material.color = Utilities.ToColor(newColor);
    }

    private void Start()
    {
        material.color = Utilities.ToColor("white");
    }

    private void OnApplicationQuit()
    {
        material.color = Utilities.ToColor("white");
    }
}
