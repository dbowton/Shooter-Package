using System;
using UnityEngine;

public class SemiAuto : Gun
{
    private bool fired = false;

    public override void Start()
    {
        base.Start();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }


    protected override void Shoot()
    {
        if(!fired)
        {
            heat = Mathf.Min(recoilPattern.Length - 1, heat + 1);
            heatCooldownTimer = heatCooldown;

            SimulateBullet();

            fired = true;
        }
    }

    public override void Released()
    {
        fired = false;
    }

    public override Type GetLowerType()
    {
        return typeof(SemiAuto);
    }
}
