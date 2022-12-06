using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MiscData : ItemData
{
    public enum ItemID
    {
        Wood,
        Stone,
        Iron,
        WeaponComponents,
        ArmourComponents
    }

    public ItemID craftingID;
    public int craftingCount = 1;

    public override Type GetBaseType()
    {
        return typeof(MiscData);
    }

    public override string GetDesc()
    {
        return Name + ": " + craftingID.ToString() + " x" + craftingCount;
    }
}
