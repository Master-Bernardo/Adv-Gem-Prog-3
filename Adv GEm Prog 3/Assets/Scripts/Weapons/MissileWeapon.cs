using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : Weapon {

    [Header("Missile Weapon")]
    public float missileReloadTime;
    public float missileMaxForce; //maximum Force applied to the Weapon
    public float missileRange; //is dependent on missile weight and max force, leave empthy for now
    public GameObject projectilePrefab;
    public float lastMisilleAttackTime = 0f;
}
