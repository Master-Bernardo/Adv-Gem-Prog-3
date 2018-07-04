using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFighter : UnitMovement
{
    [Header("Fighter Unit ")]

    protected State state;

    //attackingEnemy
    protected Vector3 currentAttackingTargetTransform;
    protected UnitMovement currentAttackingTarget;
    protected bool killedCurrentTarget = false;

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
    [Tooltip("wie gut kann der Krieger zielen")]
    public float missileAimSkill;
    [Tooltip("if true, the unit will aim perfectly with out skillbased random rotation applied to weapon")]
    public bool perfectAim = false;

    public bool steadfast = false; //for later, only some units will have this, can be disabled in game, prevents units from fleeing

    protected enum State
    {
        Idle,
        Attacking
    }

    protected enum Behaviour //TODO Unit Behaviour
    {
        Standard, //agressiv nur in Überzahl, sonst defensiv, bei großer unterzahl fliehen
        Aggressive, //greift alles an was er sieht
        Defensive,  //melle greift nur im gewissen radius an, verfolgt nicht, dreht sich immer zu den nähesten Gegner
        DefensiveStone, //schlägt nur zurück wenn er angegriffen wurde, bewegt sich nicht
        Evasive, //flieht immer von Gegnern
    }

    protected enum AttackBehaviour
    {
        Determined, //geht auf den selektierten Gegner zu, ignoriert alles andere in seiner Umgebung
        DeterminedEvasive, //versucht den selektierten Gegner anzugreifen, geht dabei anderen Gegners aus dem Weg
        Standard, //geht auf den selektierten Gegner zu, wechselt aber auf andere gegner über, falls diese näher an ihm sind
    }


    void Awake()
    {
        state = State.Idle;
    }

    protected override void Update()
    {
        base.Update();

        //check if we already killed our Target
        if(state == State.Attacking)
        {
            if (currentAttackingTarget == null)
            {
                killedCurrentTarget = true;
                //now search for another target or change state to idle
                state = State.Idle;
            }
            else
            {
                    currentAttackingTargetTransform = currentAttackingTarget.gameObject.transform.position;
                    killedCurrentTarget = false;
            

                if (weapons[selectedWeapon] is MeleeWeapon)
                {
                    MeleeAttack();
                }
                else if (weapons[selectedWeapon] is MissileWeapon)
                {
                    MissileAttack();
                }
            }
        }
        else if(state == State.Idle)
        {
            //automaticly Load when we stay in a position 
            if (!moving && weapons[selectedWeapon] is MissileWeapon)
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
        //Debug.Log("I attacked Melee");
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
                if (weapon.AmmoLeft())
                {
                    RandomRotator(weapon);
                    weapon.Shoot();
                }
                //else Debug.Log("no Ammo left");

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

    /*   ------LEave the Raycast out for now- too much Performance Drain, make it selectable by player maybe---
    if (!predictedFutureLocation) {
        //checked Raycast if we dont see enemy, directShoot = false  
        //we want to hit the collider of our target
        Debug.Log("aim");
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 direction = currentAttackingTargetTransform - transform.position;
        direction.Normalize();
        if (Physics.Raycast(weapon.transform.position + direction, direction, out hit, weapon.missileRange)) //hier mal layermask zur performanceOptimierung hinzufügen
        {
            Debug.Log(hit.collider.gameObject);
            if (hit.collider.gameObject.GetComponent<UnitMovement>() == currentAttackingTarget) directFire = true;
            else directFire = false;
        }
    }
    */

    //goto here
    AimAtPredicted:


        Vector3 distDelta = currentAttackingTargetTransform - weapon.launchPoint.transform.position;
        float launchAngle = GetLaunchAngle(
            weapon.missileLaunchVelocity,
            new Vector3(distDelta.x, 0f, distDelta.z).magnitude,          //Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)),
            distDelta.y,                                                  //currentAttackingTargetTransform.y - transform.position.y,
            directFire
        );

        if (float.IsNaN(launchAngle))
        {
            //Debug.Log("Too far from Target - NaN");
            launchAngle = 0;
            //TODO dont shoot anymore, get nearer to target, but should not happen
        }


        if (RotateWeapon(launchAngle, weapon)) _aimed = true;  //wenn wir zuende mir der Waffe gezielt haben

        //new time in air https://www.youtube.com/watch?v=jb2dWXp_tlw&t=234s&list=LLnkuTCY2XUW7UV3g2Apo5ww&index=2
        //initiallVelocityYComponent = missileLaunchVelocity * sin(launachAngle)
        //initiallVelocityYComponent = vY
        //gravity = g in positive magnitude
        //Formula: time in air = (vY + Sqr[(vY)²-4*(0.5*g)*(-(startH-finalH))]/g
        // but this works only for destinations lower than our starting keight, maybe make a abs of (startH-finalH)
        float timeInAir;
        float g = Physics.gravity.magnitude;
        float vY = weapon.missileLaunchVelocity * Mathf.Sin(launchAngle * (Mathf.PI / 180));
        //vY = 5f;
        float startH = weapon.launchPoint.transform.position.y;
        float finalH = currentAttackingTargetTransform.y;
        if (finalH < startH) {
            timeInAir = (vY + Mathf.Sqrt((float)(Mathf.Pow(vY, 2) - 4 * (0.5 * g) * (-(startH - finalH))))) / g;
        } else
        {
            //t = distanceX/initiallVeclocityXComponent
            float vX = weapon.missileLaunchVelocity * Mathf.Cos(launchAngle * (Mathf.PI / 180));
            float distanceX = Vector3.Distance(currentAttackingTargetTransform, weapon.launchPoint.transform.position);
            timeInAir = distanceX / vX;
        }

        //change the currentAttackingTargetTransform based on this time , take his velocity times this time  //predict his future location
        if (!predictedFutureLocation) {
            currentAttackingTargetTransform += currentAttackingTarget.agent.velocity * (timeInAir);
            predictedFutureLocation = true;
            goto AimAtPredicted;
        } else
        {
            base.TurnToDestination(currentAttackingTargetTransform);

            if (Quaternion.Angle(transform.rotation, wishRotation) < 5)  //hier kommt ein anderer Drehcode, weil er sonst die letzten grad vie lzu langsam dreht
            {
                agent.updateRotation = false;
                Vector3 ourPosition = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 destinationPosition = new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z);
                Quaternion perfectAimRotation = Quaternion.LookRotation(destinationPosition - ourPosition);

                transform.rotation = perfectAimRotation;
                agent.updateRotation = true;
            }


            if (Quaternion.Angle(transform.rotation, wishRotation) > 0.1)  //1 dann zielt er etwas länger aber genauer  unnecessary jetzt wo die waffe sich sowieso zum gegner dreht
            {
                _aimed = false;  //wishRotation vom Parent, wenn beide sich um mehr als 1 grad unterscheiden, dann haben wir uns noch nicht zum Gegner gedreht
            }
        }

        //now it should perfectly hit, so we apply a skillbased random rotator function
       

        return _aimed;
    }

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
            theta = Mathf.Atan((Mathf.Pow(speed, 2) + Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(distance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravityConstant * distance));
        }
        
        return (theta*(180/Mathf.PI));  //change into degrees
    }
    
    private bool RotateWeapon(float launchAngle,MissileWeapon weapon)
    {
        launchAngle = -launchAngle; //cause localTransform goes in the other direction
        float yTilt = 0f; ; //we when the back is on the side of the Units it also needs to aim directly at the enemy - trigonometrie cos(a) = b/c, now theres a etter way
       
        Quaternion angleOfWeapon = Quaternion.LookRotation((currentAttackingTargetTransform - weapon.transform.position));
        Quaternion angleOfUnit = Quaternion.LookRotation((currentAttackingTargetTransform - transform.position));
        yTilt = Quaternion.Angle(angleOfWeapon, angleOfUnit);
       
        Quaternion wishedRotation = Quaternion.Euler(transform.rotation.eulerAngles.x  + launchAngle, transform.rotation.eulerAngles.x - yTilt, transform.rotation.eulerAngles.z);
        weapon.transform.localRotation = Quaternion.RotateTowards(weapon.transform.localRotation, wishedRotation, Time.deltaTime*weapon.aimSpeed);
        if (weapon.transform.localRotation == wishedRotation) return true;
        return false;
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
                //Debug.Log("stoppeWaffeLaden/Spannen ");
                StopCoroutine("LoadWeapon");
                weapon.isPreparingWeapon = false;
            }
            else if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Drawable)
            {
                //StopCoroutine("WeaponSpannen");
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
        //Debug.Log("Waffe geladennnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn ");
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

    private void RandomRotator(MissileWeapon weapon)
    {
        if (!perfectAim)
        {
            float step = 1 / missileAimSkill * 25; //5 ist grad um die wir rotieren
            if (aimed)
            {
                weapon.transform.Rotate(
                    Random.Range(-step, step),
                    Random.Range(-step, step),
                    Random.Range(-step, step));

            }
        }
    }
}


