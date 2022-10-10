using UnityEngine;

public class forwardTesters : MonoBehaviour
{
    [SerializeField] GameObject testObjectTransform;
    [SerializeField] float distance = 0.5f;

    void Update()
    {
        gameObject.transform.position = testObjectTransform.transform.position + (testObjectTransform.transform.forward * distance);   
    }
}
