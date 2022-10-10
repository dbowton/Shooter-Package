using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] bool startActive = true;
    [SerializeField] UnityEngine.Rendering.ShadowCastingMode startMode;

    [SerializeField] List<Renderer> modelRenderers;

    public Animator animator;
    [SerializeField] List<Overrides> overrides;

    private void Start()
    {
        SetActive(startActive, startMode);
    }

    [System.Serializable]
    public class Overrides
    {
        public int animationLayer;
        public string animationClip;
        public List<Offset> jointValues;
    }

    [System.Serializable]
    public class Offset
    {
        public Transform transform;
        public Vector3 offset;
    }

    public void SetActive(bool isActive, UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On)
    {
        gameObject.SetActive(isActive);
        if (!isActive) return;

        foreach (var mr in modelRenderers)
            mr.shadowCastingMode = shadowCastingMode;
    }

    public void SetOverrides()
    {
        Dictionary<Transform, Vector3> overridenValues = new Dictionary<Transform, Vector3>();
        foreach (var o in overrides)
        {
            if(animator.layerCount > o.animationLayer && 
                animator.GetCurrentAnimatorClipInfo(o.animationLayer).Length > 0 && 
                o.animationClip.Equals(animator.GetCurrentAnimatorClipInfo(o.animationLayer)[0].clip.name))
            {
                foreach(var j in o.jointValues)
                {
                    if(overridenValues.ContainsKey(j.transform))
                    {
                        overridenValues[j.transform] += j.offset;
                    }
                    else
                    {
                        overridenValues.Add(j.transform, j.offset);
                    }
                }
            }
        }

        foreach(var k in overridenValues.Keys)
            k.localEulerAngles += overridenValues[k];
    }
}
