using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFighter : UnitMovement
{
    [Header("Fighter Unit ")]

    protected State state;
    protected Vector3 currentAttackingTargetTransform;
    protected UnitMovement currentAttackingTarget;

    //new values with Weapons:
    public Weapon[] weapons;
    public int selectedWeapon = 1;
    //if(weapons[0] is MissileWeapon) blablabla

    //For unit controlls missile
    public bool directFire = true; //we can shoot in 2 angles, the units switch automaticly, but we can also do this manually
    [Tooltip("wie groß kann der winkelfehler Sein bevor man sagt, man hat fertiggeaimt")]
    private float aimTolerance;
    [Tooltip("is true falls der Krieger fertiggezielt hat")]
    [SerializeField()]
    private bool aimed = false;

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

        if(state==State.Attacking) currentAttackingTargetTransform = currentAttackingTarget.gameObject.transform.position;

        if (state == State.Attacking && weapons[selectedWeapon] is MeleeWeapon)
        {
            MeleeAttack();
        }
        else if (state == State.Attacking && weapons[selectedWeapon] is MissileWeapon)
        {
            MissileAttack();
        }

        //automaticly Load when we stay in a position 
        if (agent.velocity.magnitude < 0.1) { 
            if(state == State.Idle && weapons[selectedWeapon] is MissileWeapon)
            {
                MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon;
                if (!weapon.weaponReadyToShoot && !weapon.isPreparingWeapon && weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable) StartCoroutine("LoadWeapon");
            }
        }
    }

    public override void Attack(UnitMovement target)
    {
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
        MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon; //cast Notwendig

        //wenn laudable - dann lade hier falls nicht geladen ist, wir laden schon bevor wir in Range sind
        if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable ) 
        {
            if (!weapon.weaponReadyToShoot && !weapon.isPreparingWeapon) StartCoroutine("LoadWeapon");
        }

        if (Vector3.Distance(transform.position, currentAttackingTargetTransform) < weapon.missileRange) // wenn wir im Range sind
        {
            agent.isStopped = true;
            aimed = (Aim(weapon));

            if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Drawable) //den bogen/Wurfspeer spannen wir erst wenn wir in range sind
            {
                if (!weapon.weaponReadyToShoot && !weapon.isPreparingWeapon) StartCoroutine("WeaponSpannen");
            }

            if (aimed && weapon.weaponReadyToShoot)
            {
                MissileShoot(weapon);
                weapon.weaponReadyToShoot = false; //nach dem Schuss müssen wir nochmal laden oder bogen spannen
            }
               
        }else
        {
            SetDestinationAttack(currentAttackingTargetTransform);
        }
        
        
    }

    bool Aim(MissileWeapon weapon) //returns true if aiming is finished -
    {
        bool predictedFutureLocation = false;
        bool _aimed = false;
        
        //checked Raycast if we dont see enemy, directShoot = false  //maybe add later //TODO

        //goto here
        AimAtPredicted:


        Vector3 distDelta = currentAttackingTargetTransform - transform.position;
        float launchAngle = GetLaunchAngle(
            weapon.missileLaunchVelocity,
            new Vector3(distDelta.x, 0f, distDelta.z).magnitude,          //Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)),
            distDelta.y,                                                  //currentAttackingTargetTransform.y - transform.position.y,
            directFire
        );

        if (float.IsNaN(launchAngle))
        {
            Debug.Log("Too far from Target - NaN");
            launchAngle = 0;
            //TODO dont shoot anymore, get nearer to target, but should not happen
        }

        if(RotateWeapon(launchAngle,weapon)) _aimed = true;  //wenn wir zuende mir der Waffe gezielt haben

        

        //now calculate how long this would take

        //time of flight in seconds = 2*initiallvelocity*sin(launchAngle)/gravitymagnitude
        float timeInAir = (2 * weapon.missileLaunchVelocity * Mathf.Sin(launchAngle * (Mathf.PI / 180))) / Physics.gravity.magnitude; //Mathf.sins takes angle in radians
        //Debug.Log("launchAngle: " + launchAngle);
        Debug.Log("time in air: " + timeInAir);
        //Debug.Log(currentAttackingTarget.agent.velocity);

        //change the currentAttackingTargetTransform based on this time , take his velocity times this time  //predict his future location
        if (!predictedFutureLocation) {
            Debug.Log("currentAttackingTargetTransform: "+ currentAttackingTargetTransform);
            currentAttackingTargetTransform += currentAttackingTarget.agent.velocity * timeInAir;
            Debug.Log("PredictedcurrentAttackingTargetTransform: " + currentAttackingTargetTransform);
            predictedFutureLocation = true;
            goto AimAtPredicted;
        }else
        {
            base.TurnToDestination(currentAttackingTargetTransform);

            if (Quaternion.Angle(transform.rotation, wishRotation) > 1)  //1 dann zielt er etwas länger aber genauer
            {
                _aimed = false;  //wishRotation vom Parent, wenn beide sich um mehr als 5 grad unterscheiden, dann haben wir uns noch nicht zum Gegner gedreht
            }
        }



        //DebuggingBLock:
        /*Debug.Log("distance" + Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)));
         Debug.Log("heightDifference" + (currentAttackingTargetTransform.y - transform.position.y));
         Debug.Log("angle" + GetLaunchAngle(50,400,-200,false));
         Debug.Log("gravity" + Physics.gravity.magnitude);
         Debug.Log("math Pow test" + Mathf.Pow(2, 2));
         Debug.Log("squereRoot Test" + Mathf.Sqrt(4));*/
        //always use max Force?
        //getAimVector




        //TODO now it should perfectly hit, so we apply a skillbased random rotator function


        return _aimed;
    }

    //Formel von  https://gamedev.stackexchange.com/questions/53552/how-can-i-find-a-projectiles-launch-angle
    float GetLaunchAngle(float speed, float distance, float heightDifference, bool directShoot)
    {
        //directShoot i true dann nehmen wir die niedrigere Schussbahn, wenn false, dann eine kurvigere die mehr nach oben geht
        float theta = 0f;
        float gravityConstant = Physics.gravity.magnitude;
        distance = distance - 1.5f; // to correct it, its always a bit too far idkw 
        if (directShoot) {
            theta = Mathf.Atan((Mathf.Pow(speed, 2) - Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(distance,2) + 2*heightDifference*Mathf.Pow(speed,2))))/(gravityConstant*distance)) ;
        }
        else
        {
            theta = Mathf.Atan((Mathf.Pow(speed, 2) + Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(distance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravityConstant * distance));
        }
        
        return (theta*(180/Mathf.PI));  //change into degrees
    }
    
    private bool RotateWeapon(float launchAngle,MissileWeapon weapon)
    {
        launchAngle = -launchAngle; //cause localTransform goes in the other direction
        float yTilt = 0f; ; //we when the back is on the side of the Units it also needs to aim directly at the enemy - trigonometrie cos(a) = b/c
       
        float b = (weapon.transform.position  - currentAttackingTargetTransform).magnitude; //y vielleich auslassen  w
        float c = (transform.position - currentAttackingTargetTransform).magnitude;
        if(b<c) yTilt = Mathf.Acos(b / c);  // sonst gibts fehler beim rotieren, wenn wir nicht auf den Gegner gucken
        
        Quaternion wishedRotation = Quaternion.Euler(transform.rotation.x  + launchAngle, transform.rotation.y -yTilt , transform.rotation.z);
        weapon.transform.localRotation = Quaternion.RotateTowards(weapon.transform.localRotation, wishedRotation, Time.deltaTime*weapon.aimSpeed);
        if (weapon.transform.localRotation == wishedRotation) return true;
        return false;
    }
    

    private void MissileShoot(MissileWeapon weapon)
    {
        //TODO dies in die weapon auslagern
        Debug.Log("I shot the Sherif!!");
        Rigidbody missileRb = Instantiate(weapon.projectilePrefab, weapon.transform.position + weapon.transform.forward, weapon.transform.rotation).GetComponent<Rigidbody>();
        //missileRb.AddForce(weapon.transform.forward * weapon.missileMaxForce);
        missileRb.velocity = weapon.transform.forward * weapon.missileLaunchVelocity;
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
        if (weapons[selectedWeapon] is MissileWeapon)
        {

            MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon;
            if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable)
            {
                Debug.Log("stopCouroutine: " + agent.velocity.magnitude);
                StopCoroutine("LoadWeapon");
                weapon.isPreparingWeapon = false;
            }
            else if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Drawable)
            {
                StopCoroutine("WeaponSpannen");
                weapon.isPreparingWeapon = false;
            }
        }

    }


    //ändert den bool isReadyToShoot nach der drawTime

    IEnumerator LoadWeapon()
    {
        MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon;
        weapon.isPreparingWeapon = true;
        yield return new WaitForSeconds(weapon.missileReloadTime);
        weapon.isPreparingWeapon = false;

        weapon.weaponReadyToShoot = true;
        Debug.Log("Waffe geladennnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn ");
    }
    

    //ändert den bool isReadyToShoot nach der loadTime

    IEnumerator WeaponSpannen()
    {
        MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon;
        weapon.isPreparingWeapon = true;
        yield return new WaitForSeconds(weapon.missileReloadTime);
        weapon.isPreparingWeapon = false;
        weapon.weaponReadyToShoot = true;
    }

    //TODO laster when we dont have enough amunition for a weapon we need to communicate this somehow
    private void OutOfAmmunition()
    {

    }
}


