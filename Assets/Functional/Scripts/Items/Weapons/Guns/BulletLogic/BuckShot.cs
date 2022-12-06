using System;
using System.Collections.Generic;
using UnityEngine;

public class BuckShot : BulletLogic
{
    [SerializeField] private int pelletCount = 1;
    [Tooltip("Standard Deviation after 10 units")]
    [SerializeField] private Vector2 stdPelletDeviation = Vector2.zero;

    protected void OnValidate()
    {
        if(gameObject.transform.TryGetComponent<Gun>(out Gun gun))
        {
            gun.Validate();
        }

        if (pelletCount < 1)
            pelletCount = 1;

        if (stdPelletDeviation.x < 0 || stdPelletDeviation.y < 0)
            stdPelletDeviation = new Vector2(Mathf.Abs(stdPelletDeviation.x), Mathf.Abs(stdPelletDeviation.y));
    }

    public override void GenerateHits(Transform origin, Vector3 baseDirection, float range, WeaponData.DelegatedDamage action, Transform gunPort = null)
    {
        if (gunPort == null) gunPort = origin;

        for(int i = 0; i < pelletCount; i++)
        {
            Vector3 deviatedDirection = Vector3.zero;

            deviatedDirection.x = UnityEngine.Random.Range(-stdPelletDeviation.x, stdPelletDeviation.x);
            deviatedDirection.y = UnityEngine.Random.Range(-stdPelletDeviation.y, stdPelletDeviation.y);

            if (i == 0)
                deviatedDirection = Vector3.zero;

            deviatedDirection = (baseDirection * 10 + origin.right * deviatedDirection.x + origin.up * deviatedDirection.y).normalized;

            Debug.DrawRay(origin.position, deviatedDirection * range, Color.red, 5);

            Ray ray = new Ray(origin.position, deviatedDirection);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, range))
            {
                action(hitInfo.collider.gameObject);
                AddBullet(gunPort, hitInfo.collider.gameObject.transform);
            }
            else
            {
                AddBullet(gunPort);
            }
        }
    }
}
