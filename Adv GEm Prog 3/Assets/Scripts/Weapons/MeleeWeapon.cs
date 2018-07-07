using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {

    [Header("Melee Weapon")]
    public DamageType damageType;
    public float attackSpeed; //implemented in unit figheter 100 means 1 attack per second - without the random and skill values
    public float attackRange;

    public MeleeWeaponType meleeWeaponType;
    public MeleeWeaponAttackType[] meleeWeaponAttackType;

    public enum MeleeWeaponType
    {
        Spear,
        SpearWithShield,
        OneHandedSword,
        OneHandedSwordWithShield
    }

    public virtual void HandleAttack()
    {
        //switch statement der alle Waffen und deren angiffe durchgeht und eine Antwort sucht
    }

    public virtual void Attack()
    {
        //chooses an attack type, mostly randomly but also dependant on some factors like skill or distance to the enemy or speed
    }

    public void DrawWeapon()
    {

    }

    public void HideWeapon()
    {

    }

}
