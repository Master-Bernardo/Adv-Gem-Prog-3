using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    [Header("Basic Weapon")]
    public int damage;
    public GameObject model; //grafic Model of the weapon
    //later min nad max Damage

    void DrawWeapon()
    {

    }

    void HideWeapon()
    {

    }

}
