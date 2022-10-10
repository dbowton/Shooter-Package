using System;
using UnityEngine;

namespace Assets.Scripts.Items.Weapons
{
    public enum DamageType
    {
        FIRE,
        COLD,
        ACID,
        FORCE,
        NECROTIC,
        PSYCHIC,
        POISON,
        RADIANT,
        PIERCING,
        ELECTRIC,
        BLUNT,
        SLASHING
    }

    [Serializable]
    public class Damage
    {
        public Damage() { }
        public Damage(Damage damage)
        {
            this.damageType = damage.damageType;
            this.damage = damage.damage;
            this.duration = damage.duration;
        }

        public Damage(DamageType type, float dmg, float dur)
        {
            this.damageType = type;
            this.damage = dmg;
            this.duration = dur;
        }

        public DamageType damageType;
        [Tooltip("DPS")] public float damage;
        [Tooltip("in sec")] public float duration;
    }
}
