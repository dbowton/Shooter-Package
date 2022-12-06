using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstPersonCamera : CameraControl
{
    [SerializeField] private Transform playerHead;
    [SerializeField] PlayerManager player;

    [SerializeField] int sampleRate = 5;
    private List<Vector3> rotations = new List<Vector3>();

    protected override void OnValidate()
    {
        base.OnValidate();   

        if (sampleRate < 1)
            sampleRate = 1;
    }

    public override void UpdateCam(float _)
    {
        rotations.Add(player.Rotation);

        if (rotations.Count > sampleRate)
            rotations.RemoveAt(0);

        Vector3 averageRotation = Vector3.zero;
        averageRotation.x = rotations.Sum(x => x.x) / rotations.Count;
        averageRotation.y = rotations.Sum(x => x.y) / rotations.Count;

        SmartCamera.main.gameObject.transform.localEulerAngles = averageRotation + angleOffset;

        Vector3 posOffset = (SmartCamera.main.gameObject.transform.right * offset.x) + 
                            (SmartCamera.main.gameObject.transform.up * offset.y) + 
                            (SmartCamera.main.gameObject.transform.forward * offset.z);

        SmartCamera.main.gameObject.transform.position = playerHead.position;
        SmartCamera.main.gameObject.transform.localPosition += posOffset;
    }
}
