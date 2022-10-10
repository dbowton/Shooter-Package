using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fists : Weapon
{
    public override void Start()
    {
        base.Start();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    public override Type GetLowerType()
    {
        return typeof(Fists);
    }

    public override void Pressed()
    {

    }

    public override void Released()
    {

    }

    public override void update(bool pressed)
    {

    }
}
