using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SemiAutoData : GunData
{
    private bool fired = false;

    protected override void Shoot()
    {
        if (!fired)
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
