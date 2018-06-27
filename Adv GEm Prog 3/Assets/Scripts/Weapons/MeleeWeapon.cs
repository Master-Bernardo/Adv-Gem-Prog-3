using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {

    [Header("Melee Weapon")]
    public float attackPause; //pause between attacks
    public float attackRange;
    public float lastMeleeAttackTime = 0f;
}
