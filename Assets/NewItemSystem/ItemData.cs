using Assets.Scripts.Items.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#region Items
[System.Serializable]
public abstract class ItemData
{
    public GameObject itemPrefab;
    [HideInInspector] public GameObject spawnedObject;
    public Sprite itemIcon;
    public string Name;
    public float Value;

    public abstract string GetDesc();
    public abstract System.Type GetBaseType();
}

public abstract class ItemInfo : MonoBehaviour { }

#region Weapons
[System.Serializable]
public abstract class WeaponData : ItemData
{
    [Header("Weapon Base")]
    [Tooltip("Damage is for Each Projectile")]
    [SerializeField] protected List<Damage> damages = new List<Damage>();

    [Header("HandOffsets")]
    public Vector3 posOffset;
    public Vector3 rotOffset;

    [HideInInspector] public Character owner;

    public delegate void DelegatedDamage(GameObject gameObject, float multi = 1);
    public DelegatedDamage damage;

    virtual public void Start()
    {
        damage = Damage;
    }

    public enum WeaponType
    {
        Unarmed = -1,
        Melee = 0,
        Pistol = 1,
        Rifle = 2
    }

    public WeaponType weaponType = WeaponType.Melee;

    public abstract void Pressed();
    public abstract void Released();
    public abstract void update(bool pressed);

    public override System.Type GetBaseType()
    {
        return typeof(Weapon);
    }

    public void Damage(GameObject hitObject, float multi)
    {
        GameObject workingObject = hitObject;

        Health objHealth = null;
        while (!workingObject.TryGetComponent<Health>(out objHealth))
        {
            if (workingObject.transform.root == workingObject.transform) return;
            workingObject = workingObject.transform.parent.gameObject;
        }

        damages.ForEach(x =>
        {
            if (hitObject == null) return;

            objHealth.Damage(new Damage(x), multi * (hitObject.tag.Contains("colliderMulti: ") ? float.Parse(hitObject.tag.Substring("colliderMulti: ".Length - 1)) : 1));
        });
    }

    public abstract System.Type GetLowerType();
    public override string GetDesc()
    {
        float dmg = 0;
        foreach (var d in damages)
        {
            dmg += d.damage / ((d.duration > 0) ? d.duration : 1);
        }

        return GetLowerType().ToString() + " DMG: " + dmg + " Val: " + Value;
    }
}

public abstract class WeaponInfo : ItemInfo
{ }

#region Guns
public abstract class GunData : WeaponData
{
    protected GameObject FireableObject 
    { 
        get { return spawnedObject.GetComponent<GunInfo>().FireableObject; } 
        set { spawnedObject.GetComponent<GunInfo>().FireableObject = value; }
    }

    protected Transform viewPort 
    { 
        get { return spawnedObject.GetComponent<GunInfo>().ViewPort; } 
//        set { spawnedObject.GetComponent<GunInfo>().ViewPort = value; }
    }

    protected GameObject muzzle_flash { get { return spawnedObject.GetComponent<GunInfo>().muzzle_flash; } }

    protected Transform gunPort { get { return spawnedObject.GetComponent<GunInfo>().gunPort; } }


    public enum HeatMethod
    {
        EXACT,
        DEVIATION
    }

    [SerializeField] protected int extraAmmo;

    [HideInInspector]
    public int AmmoCount { get { return extraAmmo; } }
    public bool IsReloading { get { return reloadTimer > 0; } }

    [Header("Base")]
    [SerializeField] protected float fireInterval = 0.2f;
    [SerializeField] protected float maxRange = 100;
    [SerializeField] protected bool chamberable = true;
    [SerializeField] protected int magCapacity = 12;
    [SerializeField] protected int roundsRemaining = 11;
    [SerializeField] protected float reloadTime = 2.0f;
    [SerializeField] protected HeatMethod heatMethod = HeatMethod.EXACT;
    [Tooltip("Recoil Pattern is After 10 units")]
    [SerializeField] protected Vector2[] recoilPattern;
    [SerializeField] protected float heatCooldown;
    [SerializeField] protected float heatCooldownRate;

    protected float heat = 0;
    protected float heatCooldownTimer = 0.0f;
    protected float fireTimer = 0.0f;
    protected float reloadTimer = 0.0f;
    protected bool chambered = false;

    [HideInInspector] public int MagCapacity { get { return magCapacity; } }
    [HideInInspector] public int RoundsRemaining { get { return roundsRemaining; } }

    [Header("Sound Effects")]
    [SerializeField] protected AudioClip firedSound = null;
    [SerializeField] protected AudioClip missFiredSound = null;
    [SerializeField] protected AudioClip reloadedSound = null;

    [Header("GunParts")]
    [SerializeField] protected float muzzle_flash_time = 0.75f;

    [SerializeField] bool usePrefabForFireable;

    public override void Start()
    {
        base.Start();
    }

    public void Validate()
    {
        OnValidate();
    }

    protected void OnValidate()
    {
//        base.OnValidate();

        if (fireInterval < 0)
            fireInterval = 0;

        if (magCapacity < 1)
            magCapacity = 1;

        if (roundsRemaining < 0)
            roundsRemaining = 0;

        if (maxRange <= 0)
            maxRange = 1;

        if (reloadTime < 0)
            reloadTime = 0;

        if (roundsRemaining > magCapacity + ((chamberable) ? 1 : 0))
            roundsRemaining = magCapacity + ((chamberable) ? 1 : 0);

        if (heatCooldown < 0)
            heatCooldown = 0;

        if (heatCooldownRate < 0)
            heatCooldownRate = 0;

        if (extraAmmo < 0)
            extraAmmo = 0;

        if (recoilPattern == null || recoilPattern.Length < 1)
        {
            Array.Resize(ref recoilPattern, 1);
            recoilPattern[0] = Vector3.zero;
        }

        if (heatMethod == HeatMethod.DEVIATION)
        {
            for (int i = 0; i < recoilPattern.Length; i++)
            {
                recoilPattern[i].x = Mathf.Abs(recoilPattern[i].x);
                recoilPattern[i].y = Mathf.Abs(recoilPattern[i].y);
            }
        }

        if (spawnedObject.GetComponent<GunInfo>().gunLogic == null)
        {
            if (itemPrefab.TryGetComponent<BulletLogic>(out BulletLogic foundBulletLogic))
            {
                spawnedObject.GetComponent<GunInfo>().gunLogic = foundBulletLogic;
            }
            else
            {
                spawnedObject.GetComponent<GunInfo>().gunLogic = itemPrefab.AddComponent<SingleShot>();
            }
        }
        else
        {
            List<BulletLogic> bulletLogics = itemPrefab.transform.GetComponents<BulletLogic>().ToList();

            switch (bulletLogics.Count)
            {
                case 0:
                    itemPrefab.AddComponent<SingleShot>();
                    break;
                case 1:
                    spawnedObject.GetComponent<GunInfo>().gunLogic = bulletLogics[0];
                    break;
                default:
                    spawnedObject.GetComponent<GunInfo>().gunLogic = bulletLogics[bulletLogics.Count - 1];
                    break;
            }
        }
    }

    public override void Released() { }
    public override void Pressed() { Fire(); }

    public void Fire()
    {
        if (reloadTimer <= 0 && fireTimer <= 0)
        {
            if (roundsRemaining > 0)
            {
                if (FireableObject)
                    UnityEngine.Object.Destroy(FireableObject);

                fireTimer = fireInterval;

                Shoot();
            }
            else
            {
                Reload();
            }
        }
    }

    public virtual void Reload()
    {
        reloadTimer = reloadTime;
    }

    public override void update(bool pressed)
    {
        if (pressed) Pressed();
        else Released();

        if (fireTimer > 0) fireTimer -= Time.deltaTime;
        if (heatCooldownTimer > 0) heatCooldownTimer -= Time.deltaTime;
        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0)
            {
                if (spawnedObject.GetComponent<GunInfo>().gunLogic is Projectile)
                {
                    FireableObject = UnityEngine.Object.Instantiate((spawnedObject.GetComponent<GunInfo>().gunLogic as Projectile).projectilePrefab, gunPort);
                    if (FireableObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) UnityEngine.Object.Destroy(rigidbody);
                    if (FireableObject.TryGetComponent<Collider>(out Collider collider)) UnityEngine.Object.Destroy(collider);
                }

                if (reloadedSound)
                    AudioSource.PlayClipAtPoint(reloadedSound, spawnedObject.transform.position);

                //  max request
                int requestedAmmo = magCapacity + ((chamberable && roundsRemaining > 0) ? 1 : 0) - roundsRemaining;
                requestedAmmo = Mathf.Min(requestedAmmo, extraAmmo);

                roundsRemaining += requestedAmmo;
                extraAmmo -= requestedAmmo;
            }
        }
    }

    protected abstract void Shoot();

    protected void SimulateBullet()
    {
        if (roundsRemaining > 0)
        {
            float xDev;
            float yDev;

            switch (heatMethod)
            {
                case HeatMethod.EXACT:
                    xDev = recoilPattern[(int)heat].x;
                    yDev = recoilPattern[(int)heat].y;
                    break;
                case HeatMethod.DEVIATION:
                    xDev = UnityEngine.Random.Range(-recoilPattern[(int)heat].x, recoilPattern[(int)heat].x);
                    yDev = UnityEngine.Random.Range(-recoilPattern[(int)heat].y, recoilPattern[(int)heat].y);
                    break;
                default:
                    throw new InvalidOperationException("How?");
            }

            Vector3 dir = (viewPort.forward * 10 + viewPort.right * -xDev + viewPort.up * yDev).normalized;

            if (firedSound)
                AudioSource.PlayClipAtPoint(firedSound, spawnedObject.transform.position);

            if (muzzle_flash)
            {
                //                print("fired");
                GameObject flash = UnityEngine.Object.Instantiate(muzzle_flash, gunPort);

                if (flash.TryGetComponent<ParticleSystem>(out ParticleSystem ps)) ps.Play();

                UnityEngine.Object.Destroy(flash, muzzle_flash_time);
            }

            roundsRemaining--;

            spawnedObject.GetComponent<GunInfo>().gunLogic.GenerateHits(viewPort, dir, maxRange, damage, gunPort);
        }
        else
        {
            if (missFiredSound)
                AudioSource.PlayClipAtPoint(missFiredSound, spawnedObject.transform.position);
        }
    }
}

#endregion
#endregion
#endregion