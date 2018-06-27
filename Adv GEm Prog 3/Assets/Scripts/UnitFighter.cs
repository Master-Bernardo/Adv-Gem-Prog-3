using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFighter : UnitMovement
{
    [Header("Fighter Unit ")]
    //all this will not be used anymore
 
    
    
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
        base.Update();
        if (state == State.Attacking && weapons[selectedWeapon] is MeleeWeapon)
        {
            Debug.Log("MeleeAttack");
            MeleeAttack();
        }
        else if (state == State.Attacking && weapons[selectedWeapon] is MissileWeapon)
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
            if (Time.time > weapon.lastMeleeAttackTime + weapon.attackPause)
            {
                MeleeHit(weapon.damageType,weapon.damage);
                weapon.lastMeleeAttackTime = Time.time;
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
        MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon; //cast Notwendig

        if (Vector3.Distance(transform.position, currentAttackingTargetTransform) < weapon.missileRange)
        {
            Debug.Log("in missile range");
            agent.isStopped = true;
            if (Time.time > weapon.lastMisilleAttackTime + weapon.missileReloadTime)
            {
                //if (Aim(weapon)) { //nur wenn wir zielen können, schießen wir
                Aim(weapon);
                MissileShoot(weapon);
                
                
                weapon.lastMisilleAttackTime = Time.time;
            }
        }else
        {
            SetDestinationAttack(currentAttackingTargetTransform);
        }
        
        
    }

    void Aim(MissileWeapon weapon) //returns true if aiming is finished - do this erst wen alles andere läuft
    {
        base.TurnToDestination(currentAttackingTargetTransform);
        //Aim igher / rotate weapon = GetLaunchAngle();

        //always use max Force?
        //getAimVector
        //now it should perfectly hit, so we apply a skillbased random rotator function
        //return !manualTurning; //cause when its true, he is just turning only for now, later we also have wish Angle
    }

    //TODO Formel anwenden und Raycast - welcher sagt welcher winkel genommen wird
    float GetLaunchAngle(float speed, float distance, float heightDifference)
    {
        return 0f;
    }
    /*
    Vector3 GetAimVector(Vector3 start, Vector3 destination)
    {
        Vector3 distDelta = destination - start;
        float distance = new Vector3(distDelta.x, 0f, distDelta.z).magnitude;
        float heightDifference = distDelta.y;
        float launchAngle = GetLaunchAngle(maxMisileVelocity, distance, heightDifference);
        return transform.TransformDirection(new Vector3(0f, Mathf.Sin(launchAngle), Mathf.Cos(launchAngle)) * maxMissileVelocity);
        //wir haben den vektor wieviel nach oben - jetzt brauchen wir das in Wold coordinates
    }
    */
    private void MissileShoot(MissileWeapon weapon)
    {
        //TODO
        Debug.Log("I shot the Sherif!!");
        Rigidbody missileRb = Instantiate(weapon.projectilePrefab, weapon.transform.position + weapon.transform.up, weapon.transform.rotation).GetComponent<Rigidbody>();
        missileRb.AddForce(transform.forward * weapon.missileMaxForce);
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


