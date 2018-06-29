using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : Weapon {

    [Header("Missile Weapon")]
    [Tooltip("either draw time for a bow or load time for a crossbow or gun")]
    public float missileReloadTime;
    [Tooltip("launchVelocity in m/s")]
    public float missileLaunchVelocity;
    [Tooltip("Gets calculatet automaticly, based on velocity dont write anything in here")]
    public float missileRange = 0; 
    public GameObject projectilePrefab;
    [Tooltip("how fast does our warrior aim - 500 is quite good")]
    public float aimSpeed = 20;
    [Tooltip("dont touch, is weapon loaded - or is the bow drawn and ready to release?")]
    public bool weaponReadyToShoot = false; 
    [Tooltip("dont touch, are we loading our weapon or currently drawing our bow?")]
    public bool isPreparingWeapon = false;

    [Tooltip("drawable for bow or throwing axes, loadable for crossbws or guns")]
    public MissileWeaponType missileWeaponType;

    public enum MissileWeaponType
    {
        Drawable,
        Loadable,
    }

    private void Start()
    {
        //https://en.wikipedia.org/wiki/Range_of_a_projectile
        missileRange = Mathf.Pow(missileLaunchVelocity, 2) / Physics.gravity.magnitude * Mathf.Sin(2*45); // i ignore y for now and test the distance with a 45 angle , so pipapo
    }


}
