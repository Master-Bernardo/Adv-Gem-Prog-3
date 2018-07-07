using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MeleeWeapon
{

    private void Awake()
    {
        meleeWeaponType = MeleeWeaponType.OneHandedSword;
        meleeWeaponAttackType = new MeleeWeaponAttackType[]{

                                    MeleeWeaponAttackType.S_Thrust_High, //also speer andersrum halten
                                    MeleeWeaponAttackType.S_Thrust_Low,
                                    MeleeWeaponAttackType.S_Charge_OS, //OS - ohne Schield
                                    //wenn kein Schild
                                    MeleeWeaponAttackType.S_Blunt_Hit, //alos mit dem Unteren nicht spitzen teil
                                    //with shield
                                    MeleeWeaponAttackType.S_Charge_WS // WS with shield
    };
    }



}
