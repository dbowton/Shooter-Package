using Assets.Scripts.Items.Weapons;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
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

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    public override System.Type GetBaseType()
    {
        return typeof(Weapon);
    }

    public void Damage(GameObject hitObject, float multi)
    {
        if(hitObject.TryGetComponent<Health>(out Health objHealth))
        {
            damages.ForEach(x =>
            {
                if (hitObject == null) return;

                objHealth.Damage(new Damage(x), multi * (hitObject.tag.Contains("colliderMulti: ") ? float.Parse(hitObject.tag.Substring("colliderMulti: ".Length - 1)) : 1));
            });
        }
        else if (hitObject.transform.root.TryGetComponent<Health>(out Health rootHealth))
        {
            damages.ForEach(x =>
            {
                if (hitObject == null) return;
                
                rootHealth.Damage(new Damage(x), multi * (hitObject.tag.Contains("colliderMulti: ") ? float.Parse(hitObject.tag.Substring("colliderMulti: ".Length - 1)) : 1));
            });
        }
    }

    public abstract System.Type GetLowerType();

    public override string GetDesc()
    {
        float dmg = 0;
        foreach(var d in damages)
        { 
            dmg += d.damage / ((d.duration > 0) ? d.duration : 1);
        }

        return GetLowerType().ToString() + " DMG: " + dmg + " Val: " + value;
    }
}
