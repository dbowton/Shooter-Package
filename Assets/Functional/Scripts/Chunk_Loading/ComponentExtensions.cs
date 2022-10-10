using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadable
{
    public void WriteComponent();
    public Component ReadInComponent(List<string> text, ref int readPosition);
}

public static class ComponentExtensions
{
    public static bool isLoadable(this Component component)
    {
        if (component is Transform) return true;
        if (component is Rigidbody) return true;
        
        return (component is ILoadable);
    }

    public static Transform ReadInComponent(this Transform transform, List<string> text, ref int readPosition)
    {
        (Vector3 position, Vector3 rotation, Vector3 scale) = ReadInTransform(text, ref readPosition);

        transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        transform.localScale = scale;

        return transform;
    }

    public static Rigidbody ReadInComponent(this Rigidbody rigidbody, List<string> text, ref int readPosition)
    {
        (bool kinematic, Vector3 velocity, Vector3 pos, Vector3 rotation) = ReadInRigidbody(text, ref readPosition);

        rigidbody.isKinematic = kinematic;
        rigidbody.position = pos;
        rigidbody.velocity = velocity;
        rigidbody.rotation = Quaternion.Euler(rotation);

        return rigidbody;
    }

    public static (bool, Vector3, Vector3, Vector3) ReadInRigidbody(List<string> text, ref int readPosition)
    {
        bool kinematic = bool.Parse(text[readPosition++]);
        
        Vector3 vel = Vector3.zero;
        vel.x = float.Parse(text[readPosition++]);
        vel.y = float.Parse(text[readPosition++]);
        vel.z = float.Parse(text[readPosition++]);

        Vector3 pos = Vector3.zero;
        pos.x = float.Parse(text[readPosition++]);
        pos.y = float.Parse(text[readPosition++]);
        pos.z = float.Parse(text[readPosition++]);

        Vector3 rot = Vector3.zero;
        rot.x = float.Parse(text[readPosition++]);
        rot.y = float.Parse(text[readPosition++]);
        rot.z = float.Parse(text[readPosition++]);

        return (kinematic, vel, pos, rot);
    }

    public static List<string> WriteOutRigidbody(this Rigidbody rigidbody)
    {
        List<string> objectInfo = new List<string>();

        objectInfo.Add(rigidbody.isKinematic.ToString());

        objectInfo.Add(rigidbody.velocity.x.ToString());
        objectInfo.Add(rigidbody.velocity.y.ToString());
        objectInfo.Add(rigidbody.velocity.z.ToString());

        objectInfo.Add(rigidbody.position.x.ToString());
        objectInfo.Add(rigidbody.position.y.ToString());
        objectInfo.Add(rigidbody.position.z.ToString());

        objectInfo.Add(rigidbody.rotation.x.ToString());
        objectInfo.Add(rigidbody.rotation.y.ToString());
        objectInfo.Add(rigidbody.rotation.z.ToString());

        return objectInfo;
    }

    public static List<string> WriteOutTransform(this Transform transform)
    {
        List<string> objectInfo = new List<string>();

        objectInfo.Add(transform.position.x.ToString());
        objectInfo.Add(transform.position.y.ToString());
        objectInfo.Add(transform.position.z.ToString());

        objectInfo.Add(transform.rotation.eulerAngles.x.ToString());
        objectInfo.Add(transform.rotation.eulerAngles.y.ToString());
        objectInfo.Add(transform.rotation.eulerAngles.z.ToString());

        objectInfo.Add(transform.localScale.x.ToString());
        objectInfo.Add(transform.localScale.y.ToString());
        objectInfo.Add(transform.localScale.z.ToString());

        return objectInfo;
    }

    public static (Vector3, Vector3, Vector3) ReadInTransform(List<string> text, ref int readPosition)
    {
        Vector3 position = Vector3.zero;
        position.x = float.Parse(text[readPosition++]);
        position.y = float.Parse(text[readPosition++]);
        position.z = float.Parse(text[readPosition++]);

        Vector3 rotation = Vector3.zero;
        rotation.x = float.Parse(text[readPosition++]);
        rotation.y = float.Parse(text[readPosition++]);
        rotation.z = float.Parse(text[readPosition++]);

        Vector3 scale = Vector3.zero;
        scale.x = float.Parse(text[readPosition++]);
        scale.y = float.Parse(text[readPosition++]);
        scale.z = float.Parse(text[readPosition++]);

        return (position, rotation, scale);
    }
}
