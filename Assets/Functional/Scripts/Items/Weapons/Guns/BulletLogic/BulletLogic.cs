using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletLogic : MonoBehaviour
{
    public bool useVisualHit = false;
    public GameObject projectilePrefab;
    public AudioClip hitSound;
    public float speed;
    public float projectileTime;
    public abstract void GenerateHits(Transform origin, Vector3 baseDirection, float range, WeaponData.DelegatedDamage action, Transform gunPort = null);

    List<(GameObject bullet, float destructionTime, Transform target)> bullets = new List<(GameObject bullet, float destructionTime, Transform target)>();

    public void AddBullet(Transform start, Transform target = null)
    {
//        if (target) print(target.gameObject.name);

        if(useVisualHit)
        {
            GameObject bullet = Instantiate(projectilePrefab, start);
            bullet.transform.parent = null;

            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(start.forward * speed, ForceMode.VelocityChange);
            }

            bullets.Add((bullet, projectileTime, target));
        }
    }

    public virtual void Update()
    {
        for(int i = 0; i < bullets.Count;)
        {
            if (bullets[i].target && Vector3.Distance(bullets[i].bullet.transform.position, bullets[i].target.position) < 0.05f)
            {
                bullets[i] = (bullets[i].bullet, 0, bullets[i].target);
            }

            bullets[i] = (bullets[i].bullet, bullets[i].destructionTime - Time.deltaTime, bullets[i].target);

            if(bullets[i].destructionTime <= 0)
            {
                Destroy(bullets[i].bullet);
                bullets.RemoveAt(i);
                continue;
            }

            i++;
        }
    }
}
