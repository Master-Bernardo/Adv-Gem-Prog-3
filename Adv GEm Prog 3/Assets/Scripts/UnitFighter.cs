using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFighter : UnitMovement
{
    [Header("Fighter Unit ")]
    //all this will not be used anymore
 
    protected float lastMeleeAttackTime = 0f;
    
    protected State state;
    protected Vector3 currentAttackingTargetTransform;
    protected UnitMovement currentAttackingTarget;

    //new values with Weapons:
    public Weapon[] weapons;
    public int selectedWeapon = 0;
    //if(weapons[0] is MissileWeapon) blablabla

    public bool steadfast = false; //for later, only some units will have this, can be disabled in game, prevents units from fleeing

    protected enum State
    {
        Idle,
        Attacking
    }

    protected enum Behaviour //TODO Unit Behaviour
    {
        Standard, //agressiv nur in Überzahl, sonst defensiv, bei großer unterzahl fliehen
        Aggressive,
        Defensive,  //melle greift nur im meleeradius an, verfolgt nicht, wenn angegriffen wird von anderen Melee, geht es so nah ran, dass es sie angreifen kann/bzw ganz kleiner agressiver Angriffsradius
        Evasive, //flieht immer von Gegnern
    }


    void Awake()
    {
        state = State.Idle;
    }

    protected override void Update()
    {
        // Debug.Log(gameObject + " " + state);
        base.Update();
        if (state == State.Attacking && weapons[selectedWeapon] is MeleeWeapon)
        {
            Debug.Log("MeleeAttack");
            MeleeAttack();
        }else if (state == State.Attacking && weapons[selectedWeapon] is MissileWeapon)
        {
            MissileAttack();
            Debug.Log("MisilleAttack");
        }
    }

    public override void Attack(UnitMovement target)
    {
        Debug.Log("Attack");
        currentAttackingTargetTransform = target.gameObject.transform.position;
        currentAttackingTarget = target;
        state = State.Attacking;
    }


    public void MeleeAttack()
    {
        SetDestinationAttack(currentAttackingTargetTransform);
        MeleeWeapon weapon = weapons[selectedWeapon] as MeleeWeapon; //cast Notwendig

        if (Vector3.Distance(transform.position, currentAttackingTargetTransform) < weapon.attackRange)
        {
            agent.isStopped = true;
            if (Time.time > lastMeleeAttackTime + weapon.attackPause)
            {
                MeleeHit(weapon.damageType,weapon.damage);
                lastMeleeAttackTime = Time.time;
            }
        }
    }

    private void MeleeHit(DamageType damageType, int damage)
    {
        Debug.Log("I attacked Melee");
        currentAttackingTarget.GetDamage(damageType, damage);
    }

    private void MissileAttack()
    {
        //TODO
    }

    private void MissileShoot()
    {
        //TODO
        Debug.Log("I shot the Sherif!!");
    }



    public override void SetDestination(Vector3 destination)  //set destination with rmb
    {
        base.SetDestination(destination);
        AbortAttack();
        agent.isStopped = false;
    }

    private void SetDestinationAttack(Vector3 destination)  // automatic set destination of attack rmb was licked
    {
        base.SetDestination(destination);
        agent.isStopped = false;
    }

    private void AbortAttack()
    {
        if (state == State.Attacking)
        {
            state = State.Idle;
        }
    }
}


