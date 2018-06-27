using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitProto : UnitMovement {

    public float attackRate; //the same like fireRate bot for melee - the smaller the better
    protected float lastAttackTime = 0f;
    public int damage; //for now always the same damage on hit
    public DamageType damageType;
    public float attackRange;
    protected State state;
    protected Vector3 currentAttackingTargetTransform;
    protected UnitMovement currentAttackingTarget;
    protected bool meleeWeaponDrawn = true; // only if its drawn , we can attack

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

	
	void Awake () {
        state = State.Idle;
        
	}

    protected override void Update()
    {
       // Debug.Log(gameObject + " " + state);
        base.Update();
        if(state == State.Attacking && meleeWeaponDrawn)
        {
            MeleeAttackUpdate();
        }
    }
    public void MeleeAttackUpdate()
    {
        SetDestinationAttack(currentAttackingTargetTransform);
        if (Vector3.Distance(transform.position, currentAttackingTargetTransform) < attackRange)
        {
            agent.isStopped = true;
            if (Time.time > lastAttackTime + attackRate)
            {
                MeleeHit();
                lastAttackTime = Time.time;
            }
        }
    } 


    public override void Attack(UnitMovement target)
    {
        Debug.Log("Attack");
        currentAttackingTargetTransform = target.gameObject.transform.position;
        currentAttackingTarget = target;
        state = State.Attacking;
    }

    public override void SetDestination(Vector3 destination)
    {
        base.SetDestination(destination);
        AbortAttack();
        agent.isStopped = false;
    }

    private void SetDestinationAttack(Vector3 destination)
    {
        base.SetDestination(destination);
        agent.isStopped = false;
    }

    private void MeleeHit()
    {
        Debug.Log("I attacked");
        currentAttackingTarget.GetDamage(damageType, damage);
    }

    private void AbortAttack()
    {
        if(state == State.Attacking) { 
            state = State.Idle;
        }
    }
}
