using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : BulletLogic
{
    [SerializeField] GameObject hitPrefab;
    [SerializeField] float hitTime;

    private List<ProjectileInfo> projectiles = new List<ProjectileInfo>();

    [SerializeField] bool isAOE;

//    [SerializeField] AudioSource hitSound;

    [SerializeField] float hitRange;
    [SerializeField] float damageRange;
    [SerializeField] float pushBack;
    Weapon.DelegatedDamage action;

    private void OnValidate()
    {
        if(projectilePrefab.transform.Find("HitPoint") == null)
        {
            print("ProjectilePrefab: " + projectilePrefab.name + " is Missing the required Child HitPoint");
            projectilePrefab = null;
        }
    }

    public class ProjectileInfo
    {
        public ProjectileInfo(GameObject go, float time)
        {
            projectile = go;
            projectileTimer = time;

            hitPoint = go.transform.Find("HitPoint").gameObject;
        }

        public GameObject projectile;
        public GameObject hitPoint;
        public float projectileTimer;
    }

    public override void GenerateHits(Transform origin, Vector3 baseDirection, float range, Weapon.DelegatedDamage action, Transform gunPort = null)
    {
        this.action = action;

        ProjectileInfo projectile = new ProjectileInfo(Instantiate(projectilePrefab, origin), projectileTime);

        projectile.projectile.transform.parent = null;
        projectile.projectile.GetComponent<Rigidbody>().AddForce(baseDirection * speed, ForceMode.Impulse);

        projectiles.Add(projectile);
    }

    public override void Update()
    {
        base.Update();

        for(int i = 0; i < projectiles.Count; )
        {
            List<Collider> collisions = Physics.OverlapSphere(projectiles[i].hitPoint.transform.position, hitRange).ToList();

            if (collisions.ToList().Where(x => x.gameObject.transform.root != projectiles[i].projectile.transform.root).ToList().Count > 0)
            {                
                projectiles[i].projectileTimer = 0;

                List<Collider> hits = Physics.OverlapSphere(projectiles[i].hitPoint.transform.position, damageRange).ToList();
                
                if(hitSound)
                    AudioSource.PlayClipAtPoint(hitSound.clip, projectiles[i].projectile.transform.position, hitSound.volume);

                if (isAOE)
                {
                    //  Damage
                    foreach(var hit in hits)
                    {
                        float multi = (isAOE) ? Vector3.Distance(projectiles[i].hitPoint.transform.position, hit.transform.position) : 1;

                        action(hit.gameObject, Vector3.Distance(projectiles[i].hitPoint.transform.position, hit.gameObject.transform.position));
                    }

                    //  PushBack
                    hits = Physics.OverlapSphere(projectiles[i].hitPoint.transform.position, damageRange).ToList();

                    foreach(var hit in hits)
                    {
                        if (hit.TryGetComponent<Rigidbody>(out Rigidbody rigidbody) || hit.transform.root.TryGetComponent<Rigidbody>(out rigidbody))
                        {
                            Vector3 push = rigidbody.gameObject.transform.position - projectiles[i].hitPoint.transform.position;

                            rigidbody.AddForce(push.normalized * pushBack / Mathf.Max(push.magnitude, 1), ForceMode.Impulse);
                        }
                    }

                }
                else
                {
                    action(hits.Where(x => x.gameObject.transform.root != projectiles[i].projectile).First().gameObject);
                }
            }

            projectiles[i].projectileTimer -= Time.deltaTime;
            if(projectiles[i].projectileTimer <= 0)
            {
                GameObject splosion = Instantiate(hitPrefab, projectiles[i].hitPoint.transform.position, projectiles[i].hitPoint.transform.rotation);
                splosion.transform.localScale = 2 * damageRange * Vector3.one;

                Destroy(splosion, hitTime);

                Destroy(projectiles[i].projectile);
                projectiles.RemoveAt(i);
            }
            else
                i++;
        }
    }
}
