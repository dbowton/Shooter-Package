using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : CameraControl
{
    [SerializeField] PlayerManager player;
    [SerializeField] Transform followTarget;

    [SerializeField] float posRate = 0.1f;
    [SerializeField] float posSprintMulti = 1.0f;

    [SerializeField] float minRotPercent = 0.25f;
    [SerializeField] float maxRotPercent = 0.9f;

    [SerializeField] float rotRate = 0.2f;
    [SerializeField] float rotSprintMulti = 1.0f;

    public Vector3 outerPosOffset = Vector3.zero;
    public Vector3 outerRotOffset = Vector3.zero;

    Vector3 posVel = Vector3.zero;
    Vector3 rotVel = Vector3.zero;

    public float outerOffsetWeight = 0;
    public float steps = 10;

    public float range = 15;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (posRate < 0)
            posRate = 0;

        if (rotRate < 0)
            rotRate = 0;
    }

    public override void UpdateCam(float dt)
    {
        if (outerOffsetWeight > 1) outerOffsetWeight = 1;
        if (outerOffsetWeight < 0) outerOffsetWeight = 0;

        Vector3 innerPos = followTarget.position;
        innerPos += followTarget.up * offset.y;
        innerPos += followTarget.right * offset.x;
        innerPos += followTarget.forward * offset.z;

        Vector3 outerPos = followTarget.position;
        outerPos += followTarget.up * outerPosOffset.y;
        outerPos += followTarget.right * outerPosOffset.x;
        outerPos += followTarget.forward * outerPosOffset.z;

        Vector3 pos = outerOffsetWeight * outerPos + (1 - outerOffsetWeight) * innerPos;

        pos = Vector3.SmoothDamp(Camera.main.gameObject.transform.position, pos, ref posVel, posRate * ((player.playerData.sprinting) ? posSprintMulti : 1));

        //  check for camera obstruction
        if (Physics.Raycast(followTarget.position, pos - followTarget.position, out RaycastHit hit, (followTarget.position - pos).magnitude, ~(1 << gameObject.layer)))
            pos = hit.point;

        //  check for extreme 90s
        float screenPoint = Camera.main.WorldToScreenPoint(followTarget.position).x;
        float maxWidth = Camera.main.pixelWidth;

        bool extreme90s = (screenPoint / maxWidth < minRotPercent) || (screenPoint / maxWidth > maxRotPercent);

        Vector3 rot = Vector3.SmoothDamp(Camera.main.transform.forward, (followTarget.position + followTarget.forward * range - pos).normalized, ref rotVel, rotRate * ((player.playerData.sprinting) ? rotSprintMulti : 1) * ((extreme90s) ? 0.25f : 1));
        rot.Normalize();

        Camera.main.gameObject.transform.position = pos;
        Camera.main.transform.LookAt(pos + rot * (range + offset.magnitude), Vector3.up);
    }
}
