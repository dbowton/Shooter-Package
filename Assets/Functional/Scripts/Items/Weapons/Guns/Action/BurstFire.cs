using System;
using UnityEngine;

public class BurstFire : Gun
{
    [Header("Burst Specific")]
    [SerializeField] private float burstInterval = 0.05f;
    [SerializeField] private int burstCount = 3;

    private float burstTimer = 0.0f;
    private int burstFired = 0;

    private bool firing = false;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (burstInterval < 0) burstInterval = 0;
        burstCount = Mathf.Min(burstCount, magCapacity);
    }

    protected override void Shoot()
    {
        firing = true;
    }

    public override void update(bool pressed)
    {
        base.update(pressed);
        if (burstTimer > 0) burstTimer -= Time.deltaTime;

        if(firing)
        {
            if(burstTimer <= 0)
            {
                burstTimer += burstInterval;
                burstFired++;

                heat = Mathf.Min(recoilPattern.Length - 1, heat + 1);
                heatCooldownTimer = heatCooldown;

                SimulateBullet();

                if (burstFired >= burstCount || roundsRemaining <= 0)
                {
                    firing = false;
                    burstFired = 0;
                }
            }
        }
    }

    public override Type GetLowerType()
    {
        return typeof(BurstFire);
    }
}
