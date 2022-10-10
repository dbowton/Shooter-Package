using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miscellaneous : Item
{
    public override Type GetBaseType()
    {
        return typeof(Miscellaneous);
    }

    public override string GetDesc()
    {
        return "misc";
    }
}
