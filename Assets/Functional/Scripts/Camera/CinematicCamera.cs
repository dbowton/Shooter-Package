using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : CameraControl
{
    [Serializable]
    public class KeyPoints
    {
        public Vector3 postitionChange;
        public bool useForward;
        public bool useGlobal;
        public Vector3 forward;
        public Transform lookAt;
        public float time;
    }

    [SerializeField] List<KeyPoints> transitions;
    private int index = 0;

    float timer = 0.0f;

    public override void UpdateCam(float dt)
    {
        if (index >= transitions.Count) return;

        Camera.main.transform.position += transitions[index].postitionChange / (transitions[index].time / dt);
        Camera.main.transform.forward = (transitions[index].useForward) ? transitions[index].forward : (transitions[index].lookAt.position - Camera.main.transform.position);

        timer += dt;
        if (timer > transitions[index].time)
        {
            timer = 0.0f;
            index++;
        }
    }
}
