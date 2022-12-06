using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SmartCamera : MonoBehaviour
{
    public static List<CameraControl> cameras = new List<CameraControl>();
    public static CameraControl activeCamera;
    public static Camera main;

    private void Start()
    {
        main = Camera.main;
    }

    private void Update()
    {
        CheckWeights();
    }

    public void CheckWeights()
    {
        if (cameras.Count < 1) return;

        activeCamera = cameras.OrderByDescending(x => x.weight).First();
        activeCamera.UpdateCam(Time.deltaTime);
    }
}
