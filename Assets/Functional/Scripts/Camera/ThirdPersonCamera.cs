using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// temp fix | cannot handle full y rotations | 
public class ThirdPersonCamera : CameraControl
{
    [SerializeField] PlayerManager player;

    public Vector3 outerPosOffset = Vector3.zero;
    public Vector3 outerRotOffset = Vector3.zero;

    public float outerOffsetWeight = 0;
    public float steps = 10;

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    public override void UpdateCam(float dt)
    {
        if (outerOffsetWeight > 1) outerOffsetWeight = 1;
        if (outerOffsetWeight < 0) outerOffsetWeight = 0;

        Vector3 innerPos = Vector3.zero;
        innerPos += transform.up * offset.y;
        innerPos += transform.right * offset.x;
        innerPos += transform.forward * offset.z;

        Vector3 outerPos = Vector3.zero;
        outerPos += transform.up * outerPosOffset.y;
        outerPos += transform.right * outerPosOffset.x;
        outerPos += transform.forward * outerPosOffset.z;

        Vector3 totalOffest = outerOffsetWeight * (outerPos) + (1 - outerOffsetWeight) * (innerPos);
        Vector3 pos = totalOffest + (transform.position + Vector3.up * player.GetComponent<CharacterController>().height);

        SmartCamera.main.transform.position = pos;
        SmartCamera.main.transform.forward = transform.forward;

        Vector3 angles = SmartCamera.main.transform.localEulerAngles;

        SmartCamera.main.transform.localEulerAngles = Vector3.up * angles.y;
        SmartCamera.main.transform.RotateAround(pos - transform.forward * totalOffest.z, transform.right, -player.playerData.rotation.x);

        Vector3 oldForward = SmartCamera.main.transform.forward;
        oldForward.y = -oldForward.y;
        SmartCamera.main.transform.forward = oldForward;

       //  check for camera obstruction
        if (Physics.Raycast(transform.position + Vector3.up * player.playerData.characterController.height, pos - transform.position, out RaycastHit hit, (transform.position - pos).magnitude, ~(1 << gameObject.layer)))
            SmartCamera.main.transform.position = hit.point;
    }
}
