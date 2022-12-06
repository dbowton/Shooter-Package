using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FullAutoData : GunData
{
    protected override void Shoot()
    {
        heat = Mathf.Min(recoilPattern.Length - 1, heat + 1);
        heatCooldownTimer = heatCooldown;

        SimulateBullet();
    }

    public override Type GetLowerType()
    {
        return typeof(FullAuto);
    }
}
