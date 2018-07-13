using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *   represents ammunition holders like an arrow quiver(Köcher) oder Sack mit Patronen - wei bei M&B Warband
 *   damage value is added or subtracted from the base damage of the missile Weapon
 */
public class MissileAmmo : Weapon
{
    public DamageType damageType;
    public int currentAmmo;
    public int maxAmmo;
    public float velocityModifier; //some projectiles are heavier than others and thus modify the velocity and the resulting range
    public AmmoType ammoType;


    public enum AmmoType
    {
        Arrow,
        Bolt,
        Bullet
    }

    void Start () {
        currentAmmo = maxAmmo;
	}

    public void ReplenishAmmo(int ammo)
    {
        currentAmmo += ammo;
        if (currentAmmo > maxAmmo) currentAmmo = maxAmmo;
    }
    public void UseAmmo()
    {
        currentAmmo -= 1;
    }
    
	
	
}
