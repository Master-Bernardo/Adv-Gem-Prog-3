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

    //For unit controlls 
    public bool directFire = true; //we can shoot in 2 angles, the units switch automaticly, but we can also do this manually

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
            //Debug.Log("MeleeAttack");
            MeleeAttack();
        }
        else if (state == State.Attacking && weapons[selectedWeapon] is MissileWeapon)
        {
            MissileAttack();
            //Debug.Log("MisilleAttack");
        }
    }

    public override void Attack(UnitMovement target)
    {
        //Debug.Log("Attack");
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
            //Debug.Log("in missile range");
            agent.isStopped = true;
            if (Time.time > weapon.lastMisilleAttackTime + weapon.missileReloadTime)
            {
                //if (Aim(weapon)) { //nur wenn wir zielen können, schießen wir
                if (Aim(weapon))  MissileShoot(weapon);

                weapon.lastMisilleAttackTime = Time.time;
            }
        }else
        {
            SetDestinationAttack(currentAttackingTargetTransform);
        }
        
        
    }

    bool Aim(MissileWeapon weapon) //returns true if aiming is finished - do this erst wen alles andere läuft - muss nicht perfekt geaimt sein, so ungefähr  - teste toleranz später
    {
        base.TurnToDestination(currentAttackingTargetTransform);
        //checked Raycast if we dont see enemy, directShoot = false
        Vector3 distDelta = currentAttackingTargetTransform - transform.position;
        float launchAngle = GetLaunchAngle(
            weapon.missileMaxForce,
            new Vector3(distDelta.x, 0f, distDelta.z).magnitude,          //Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)),
            distDelta.y,                                                  //currentAttackingTargetTransform.y - transform.position.y,
            directFire
        );

        if (float.IsNaN(launchAngle))
        {
            Debug.Log("Too far from Target");
            launchAngle = 0;
            //TODO dont shoot anymore, get nearer to target
        }

        RotateWeapon(launchAngle,weapon);
        
        Debug.Log("launch Angle: " +launchAngle);
        


       /*Debug.Log("distance" + Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)));
        Debug.Log("heightDifference" + (currentAttackingTargetTransform.y - transform.position.y));
        Debug.Log("angle" + GetLaunchAngle(50,400,-200,false));
        Debug.Log("gravity" + Physics.gravity.magnitude);
        Debug.Log("math Pow test" + Mathf.Pow(2, 2));
        Debug.Log("squereRoot Test" + Mathf.Sqrt(4));*/
       //always use max Force?
       //getAimVector
       //now it should perfectly hit, so we apply a skillbased random rotator function
       //return !manualTurning; //cause when its true, he is just turning only for now, later we also have wish Angle
        return true;
    }

    //TODO Formel anwenden und Raycast - welcher sagt welcher winkel genommen wird
    //Formel von  https://gamedev.stackexchange.com/questions/53552/how-can-i-find-a-projectiles-launch-angle
    float GetLaunchAngle(float speed, float distance, float heightDifference, bool directShoot)
    {
        //directShoot i true dann nehmen wir die niedrigere Schussbahn, wenn false, dann eine kurvigere die mehr nach oben geht
        float theta = 0f;
        float gravityConstant = Physics.gravity.magnitude;
        if (directShoot) {
            theta = Mathf.Atan((Mathf.Pow(speed, 2) - Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(distance,2) + 2*heightDifference*Mathf.Pow(speed,2))))/(gravityConstant*distance)) ;
        }
        else
        {
            distance = distance - 1; // to correct it, its always a bit too far idkw
            theta = Mathf.Atan((Mathf.Pow(speed, 2) + Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(distance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravityConstant * distance));

        }
        
        return (theta*180/Mathf.PI);  //change into degrees
    }
    
    private void RotateWeapon(float launchAngle,MissileWeapon weapon)
    {
        launchAngle = -launchAngle; //cause localTransform goes in the other direction
        float yTilt = 0f; ; //we when the back is on the side of the Units it also needs to aim directly at the enemy - trigonometrie cos(a) = b/c
        //b =abstand weapon-target
        //c = abstand fighter-target
        float b = (weapon.transform.position  - currentAttackingTargetTransform).magnitude; //y vielleich auslassen  w
        float c = (transform.position - currentAttackingTargetTransform).magnitude;
        if(b<c) yTilt = Mathf.Acos(b / c);  // sonst gibts fehler beim rotieren, wenn wir nicht auf den Gegner gucken
        /*Debug.Log("b" + b);
        Debug.Log("c" + c);
        Debug.Log("y Tilt: " + yTilt);
        Debug.Log("transform.rotation.y: " + transform.rotation.y);
        Debug.Log("transform.rotation.y - yTilt: " + (transform.rotation.y-yTilt));
        */
        weapon.transform.localRotation = Quaternion.Euler(transform.rotation.x  + launchAngle, transform.rotation.y -yTilt , transform.rotation.z);
    }

    //private void RotateWeapon(Vector3 launchVector)
    //{
        //rotates the weapon with a speed (later)
       // Vector3 desiredWeaponRotationEulerAngles = new Vector3(0f, 0f, 0f); //90 - launchAngle
       // weapons[selectedWeapon].transform.rotation = Quaternion.Euler(launchVector);

        //
        //wir haben den vektor wieviel nach oben - jetzt brauchen wir das in Wold coordinates
    //}

    /*private void RotateWeapon(float launchAngle, MissileWeapon weapon)
    {
         weapon.transform.TransformDirection(new Vector3(0f, Mathf.Sin(launchAngle), Mathf.Cos(launchAngle)) * weapon.missileMaxForce);

    }*/

    private void MissileShoot(MissileWeapon weapon)
    {
        //TODO
        Debug.Log("I shot the Sherif!!");
        Rigidbody missileRb = Instantiate(weapon.projectilePrefab, weapon.transform.position + weapon.transform.forward, weapon.transform.rotation).GetComponent<Rigidbody>();
        //missileRb.AddForce(weapon.transform.forward * weapon.missileMaxForce);
        missileRb.velocity = weapon.transform.forward * weapon.missileMaxForce;
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


