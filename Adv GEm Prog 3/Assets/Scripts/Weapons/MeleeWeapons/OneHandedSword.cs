using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHandedSword : MeleeWeapon
{

    private void Awake()
    {
        meleeWeaponType = MeleeWeaponType.OneHandedSword;
        meleeWeaponAttackType = new MeleeWeaponAttackType[]{ //One Handed sword
                                    MeleeWeaponAttackType.OHS_Thrust_High, // stich in Hals oder Kopf, viel Damage , nur manchmal, scherer zu verteidigen
                                    MeleeWeaponAttackType.OHS_Thrust_Low,  // stick in Bauch - große verlangsamung
                                    MeleeWeaponAttackType.OHS_Oberhau_L,
                                    MeleeWeaponAttackType.OHS_Oberhau_R,
                                    //OHS_Unterhau_L,
                                    //OHS_Unterhau_R,
                                    //OHS_Charge_Thrust,  leave out for now 
                                    //OHS_Charge_Cut,

                                    //with shield
                                    MeleeWeaponAttackType.OHS_Shield_Bash
    };
    }



}
