using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ChargeData : GunData
{
    [Header("Charge Specific")]
    [SerializeField] private float maxCharge;
    [SerializeField] private float minCharge;
    [SerializeField] private float chargeRate;
    [SerializeField] private float decayRate;
    [SerializeField] private float minShoot;
    [SerializeField] private float chargeLoss;

    private float currentCharge;
    private bool charge = false;

    public override void update(bool pressed)
    {
        base.update(pressed);

        if (charge)
            currentCharge = Mathf.Min(currentCharge + chargeRate * Time.deltaTime, maxCharge);
        else if (currentCharge > minCharge)
            currentCharge = Mathf.Max(Mathf.Min(currentCharge - decayRate * Time.deltaTime, currentCharge), minCharge);

        charge = false;
    }

    public override void Pressed()
    {
        if (currentCharge >= minShoot) Shoot();
    }

    public override void Reload()
    {
        charge = true;
    }

    protected override void Shoot()
    {
        currentCharge = (chargeLoss == 0) ? 0 : Mathf.Max(currentCharge - chargeLoss, 0);
        SimulateBullet();
    }

    public override Type GetLowerType()
    {
        return typeof(Charge);
    }
}
