using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : Weapon {

    [Header("Missile Weapon")]
    public float missileReloadTime; //either draw time for a bow or load time for a crossbow or gun
    public float missileLaunchVelocity; //launchVelocity in m/s
    public float missileRange = 0; //is dependent on missile weight and max force, leave empthy for now
    public GameObject projectilePrefab;
    //public float lastMisilleAttackTime = 0f;
    public float aimSpeed = 20;
    public bool weaponReadyToShoot = false; // is the missile weapon loaded or drawn correctly(bow)
    public bool isPreparingWeapon = false; //isLoading or is Drawing
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
