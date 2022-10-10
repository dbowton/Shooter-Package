using System;
using UnityEngine;

public class Armour : Item
{
    public enum ArmourSpot
    {
        HEAD,
        CHEST,
        LEGS,
        FEET
    }

    [SerializeField] private ArmourSpot location;
    [SerializeField] private float reduction;

    [HideInInspector] public float Reduciton { get { return reduction; } }
    [HideInInspector] public ArmourSpot Location { get { return location; } }

    public override Type GetBaseType()
    {
        return typeof(Armour);
    }

    public override string GetDesc()
    {
        return Location.ToString() + "  |   " + reduction;
    }
}
