using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitProto : UnitMovement {

    public float attackRate; //the same like fireRate bot for melee - the smaller the better
    private float lastAttackTime = 0f;
    public int damage; //for now always the same damage on hit
    public DamageType damageType;
    public float attackRange;
    private State state;
    private Vector3 currentAttackingTargetTransform;
    private UnitMovement currentAttackingTarget;

    private enum State
    {
        Idle,
        Attacking
    }

    private enum Behaviour
    {

    }

	
	void Awake () {
        state = State.Idle;
        
	}

    protected override void Update()
    {
        base.Update();
        if(state == State.Attacking)
        {
            SetDestinationAttack(currentAttackingTargetTransform);
            if (Vector3.Distance(transform.position, currentAttackingTargetTransform) < attackRange)
            {
                if(Time.time>lastAttackTime + attackRate)
                {
                    MeleeHit();
                    lastAttackTime = Time.time;
                }
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
    }

    private void SetDestinationAttack(Vector3 destination)
    {
        base.SetDestination(destination);
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
