using System.Linq;
using UnityEngine;

public abstract class CameraControl : MonoBehaviour
{
    public float weight = 0;
    public Vector3 offset = Vector3.zero;
    public Vector3 angleOffset = Vector3.zero;
    public bool isActive { get { return SmartCamera.activeCamera.Equals(this); } }

    public float Weight { get { return weight; } set { weight = value; Camera.main.transform.GetComponent<SmartCamera>().CheckWeights(); } }

    public float SetActive()
    {
        if(!SmartCamera.activeCamera != this)
            Weight = SmartCamera.activeCamera.Weight + 1;

        return Weight;
    }

    protected virtual void OnValidate()
    {
        if (!Camera.main.TryGetComponent<SmartCamera>(out _))
            Camera.main.gameObject.AddComponent(typeof(SmartCamera));
    }

    private void Awake()
    {
        if (!Camera.main.TryGetComponent<SmartCamera>(out _))
            Camera.main.gameObject.AddComponent(typeof(SmartCamera));

        SmartCamera.cameras.Add(this);
    }

    public abstract void UpdateCam(float dt);
}
