using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : Weapon {

    [Header("Missile Weapon")]
    public float reloadTime;
    public float maxForce; //maximum Force applied to the Weapon
    public float range; //is dependent on missile weight and max force, leave empthy for now
}
