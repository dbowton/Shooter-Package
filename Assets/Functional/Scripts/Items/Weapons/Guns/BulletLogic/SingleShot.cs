using System.Collections.Generic;
using UnityEngine;

public class SingleShot : BulletLogic
{
    protected void OnValidate()
    {
        if (gameObject.transform.TryGetComponent<Gun>(out Gun gun))
        {
            gun.Validate();
        }
    }

    public override void GenerateHits(Transform origin, Vector3 baseDirection, float range, WeaponData.DelegatedDamage action, Transform gunPort = null)
    {
        if (gunPort == null) gunPort = origin;

        Ray ray = new Ray(origin.position, baseDirection.normalized);


        if (Physics.Raycast(ray, out RaycastHit hitInfo, range))
        {
            Debug.DrawLine(origin.position, hitInfo.point, Color.red, 2f);
            print(hitInfo.collider.gameObject.name);
            action(hitInfo.collider.gameObject);
            AddBullet(gunPort, hitInfo.collider.gameObject.transform);
        }
        else
        {
            AddBullet(gunPort);
        }
    }
}
