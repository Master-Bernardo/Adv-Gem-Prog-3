using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeWeaponAttackType //vielleicht irgendwie als Klasse machen, dass jeder Attack type seine eigene Wahrscheinlichkeit abhängig vom Skillevel hat und sein eigenen Damage usw
{
    //One Handed sword
    OHS_Thrust_High, // stich in Hals oder Kopf, viel Damage , nur manchmal, scherer zu verteidigen
    OHS_Thrust_Low,  // stick in Bauch - große verlangsamung
    OHS_Oberhau_L,
    OHS_Oberhau_R,
    OHS_Unterhau_L,
    OHS_Unterhau_R,
    OHS_Charge_Thrust,
    OHS_Charge_Cut,

    //with shield
    OHS_Shield_Bash,

    //spear
    S_Thrust_High, //also speer andersrum halten
    S_Thrust_Low,
    S_Charge_OS, //OS - ohne Schield
    //wenn kein Schild
    S_Blunt_Hit, //alos mit dem Unteren nicht spitzen teil
    //with shield
    S_Charge_WS // WS with shield
}

