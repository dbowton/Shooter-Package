using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    float speed = 6f;
    float horizontalRotation = 50f;
    float verticalRotation = 12f;

    public void update(float dt)
    {
        if(Input.GetKey(KeyCode.UpArrow))
            transform.localEulerAngles -= Vector3.right * verticalRotation * dt;
        
        if (Input.GetKey(KeyCode.DownArrow))
            transform.localEulerAngles += Vector3.right * verticalRotation * dt;
        
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.localEulerAngles -= Vector3.up * horizontalRotation * dt;
        
        if (Input.GetKey(KeyCode.RightArrow))
            transform.localEulerAngles += Vector3.up * horizontalRotation * dt;


        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * speed * dt;

        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed * dt;

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * speed * dt;

        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * speed * dt;
    }
}
