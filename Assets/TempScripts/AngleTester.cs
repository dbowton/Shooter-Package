using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTester : MonoBehaviour
{
    private Vector3 savedRot;
    [SerializeField] bool saveRot;
    [SerializeField] Vector3 offset;

    void Update()
    {
        if(saveRot)
        {
            savedRot = transform.localEulerAngles;
            saveRot = false;

            print("saved Rotation: " + savedRot.x + ", " + savedRot.y + ", " + savedRot.z);
        }

        if(savedRot.magnitude != 0)
            transform.localEulerAngles = savedRot + offset;
    }
}
