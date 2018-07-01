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

    [Tooltip("point where our Projectiles will be launched")]
    public GameObject launchPoint;

    [Tooltip("drawable for bow or throwing axes, loadable for crossbws or guns")]
    public MissileWeaponType missileWeaponType;

    //for ammo
    public MissileAmmo.AmmoType neededAmmoType;
    public MissileAmmo selectedAmmo;


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

    public void Shoot()
    {
        selectedAmmo.UseAmmo();
        
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.transform.position, transform.rotation);
        Rigidbody missileRb = projectile.GetComponent<Rigidbody>();
        Projectile projectileScript = projectile.GetComponent<Projectile>();
    
        projectileScript.setDamage(damage + selectedAmmo.damage, selectedAmmo.damageType);
        missileRb.velocity = transform.forward * missileLaunchVelocity;

        weaponReadyToShoot = false; //nach dem Schuss müssen wir nochmal laden oder bogen spannen

    }

    public void SelectAmmo(MissileAmmo ammo)
    {
        if(ammo.ammoType == neededAmmoType)
        {
            selectedAmmo = ammo;
        }
    }

    public void DrawWeapon()
    {
        model.SetActive(true);
    }

    public void HideWeapon()
    {
        model.SetActive(false);
    }

    public bool AmmoLeft()
    {
        if (selectedAmmo.currentAmmo > 0) return true;
        else return false;
    }


}
