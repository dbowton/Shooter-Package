using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInfo : WeaponInfo
{
    public GameObject FireableObject;
    public GameObject muzzle_flash;

    public Transform ViewPort { get { return (UseCameraView) ? SmartCamera.main.transform : viewPort; } }

    [SerializeField] bool UseCameraView = false;
    [SerializeField] [Tooltip("Used for raycasting")] public Transform viewPort;
    [SerializeField] [Tooltip("Used for visual elements muzzle flashes / bullet spawning")] public Transform gunPort;
    [SerializeField] public BulletLogic gunLogic;
}
