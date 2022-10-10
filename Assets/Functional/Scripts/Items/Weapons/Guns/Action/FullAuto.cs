using System;
using UnityEngine;

public class FullAuto : Gun
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
