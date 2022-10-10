using UnityEngine;
using UnityEngine.UI;

public abstract class ActiveAbility : Ability
{
    [SerializeField] protected KeyCode button;
    protected float cooldown;

    protected Timer timer;

    [SerializeField] protected Image abilityImage;
    [SerializeField] protected RectTransform abilityBackground;

    public Color readyColor;
    public Color chargingColor;
    public Color activeColor;

    [System.Serializable]
    public abstract class GeneralStats
    {
        [Header("Active Ability Info")]
        public float duration;
        public float cooldown;

        public Sprite lvlSprite;
    }
}
