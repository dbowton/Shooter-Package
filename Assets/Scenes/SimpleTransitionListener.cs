using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTransitionListener : MonoBehaviour
{
    [SerializeField] string newScene;
    [SerializeField] float transTime = 1f;

    void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.Instance.Transition(newScene, transTime);
        }
    }
}
