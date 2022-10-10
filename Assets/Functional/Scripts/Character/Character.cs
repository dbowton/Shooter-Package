using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string Name = "Unnamed";
    public bool active = true;
    [SerializeField] protected Health health;

    internal bool die;

    private void Update()
    {
        update(Time.deltaTime);
    }

    public virtual void update(float dt) {}

    public virtual void Die()
    {
        die = true;
    }
}
