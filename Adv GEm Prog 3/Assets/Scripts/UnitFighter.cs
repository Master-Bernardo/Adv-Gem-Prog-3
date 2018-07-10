using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFighter : UnitMovement
{
    [Header("Fighter Unit ")]

    protected State state;

    //attackingEnemy
    protected Transform currentAttackingTargetTransform; //referenz auf den Transform des Objektes
    protected float predictedAttackingPositionOffset = 0f;// der current Offset um den wir TargetTransform addieren müssen
    protected UnitMovement currentAttackingTarget;

    //new values with Weapons:
    public Weapon[] weapons;
    public int selectedWeapon = 1; //if(weapons[0] is MissileWeapon) blablabla
    MissileWeapon currentSelectedMissileWeapon;
    MeleeWeapon currentSelectedMeleeWeapon;  //optimisation s owe dont always have to acess aray and cast


    #region For unit controlls missile

    [Tooltip("for directFireCheckRaycast if true directFire will be set automaticly")]
    public bool automaticDirectFire = true;
    [Tooltip("we can shoot in 2 angles, the units switch automaticly, but we can also do this manually")]
    public bool directFire = true;
    //wie groß kann der winkelfehler Sein bevor man sagt, man hat fertiggeaimt")]
    private float aimTolerance;
    //[Tooltip("is true falls der Krieger fertiggezielt hat")]
    //public bool aimed = false;
    [Tooltip("wie gut kann der Krieger zielen")]
    public float missileAimSkill;
    [Tooltip("if true, the unit will aim perfectly with out skillbased random rotation applied to weapon")]
    public bool perfectAim = false;



    private Quaternion wishedWeaponRotation = Quaternion.identity;  //we always rotate on update to this rotation

    //for optimalisation
    private bool missileAttackPrepared = false; //brauchen wir damit wir erst schießen nachdem wir zumindest einmal gezielt haben
    private bool raycastSendForThisAttack = false; //schickt vor jedem Schuss ein Raycast, wenn wir uns zum Gegner gedreht haben
    [Tooltip("intervall every this we call the prepareMissileAttackMethod")]
    public float missileAttackIntervall = 1f;
    private float nextMissileAttackTime;

    #endregion

    #region for unit controlls melee
    private float nextMeleeAttackTime = 0f;
    [Range(0,100)]
    public float meleeAttackSkill = 5f;
    [Range(0, 100)]
    public float meleeDefenseSkill = 5f;

    bool meleeAttackDestinationSet = false; //so we dont call setDestination every frame
    #endregion

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
        currentSelectedMissileWeapon = weapons[selectedWeapon] as MissileWeapon; // will be later in DrawWeapon
    }

    protected override void Update()
    {
        base.Update();

        if (state == State.Attacking)
        {
            //checkIfTarget is Dead
            if (currentAttackingTarget == null)
            {
                //now search for another target or change state to idle
                state = State.Idle;
            }
            else
            {
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
        else if (state == State.Idle)
        {
            #region automaticlyLoadWhileStanding
            if (!moving && weapons[selectedWeapon] is MissileWeapon)
            {
                MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon;
                if (!weapon.weaponReadyToShoot && !weapon.isPreparingWeapon && weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable) StartCoroutine("LoadWeapon");
            }
            #endregion
        }  
    }

    

    public override void Attack(UnitMovement target)
    {
        currentAttackingTarget = target;
        currentAttackingTargetTransform = currentAttackingTarget.gameObject.transform;
        state = State.Attacking;

        //set this for our missileUnits
        missileAttackPrepared = false;
        nextMissileAttackTime = Time.time + Random.Range(0f, missileAttackIntervall);

        //set this for meleeUnits
        meleeAttackDestinationSet = false;
    }


    public void MeleeAttack()  
    {
        MeleeWeapon currentSelectedMeleeweapon = weapons[selectedWeapon] as MeleeWeapon; //cast Notwendig

        if (Vector3.Distance(transform.position, currentAttackingTargetTransform.position) < currentSelectedMeleeweapon.attackRange)
        {
            //Debug.Log("in Range");
            TurnToDestination(currentAttackingTargetTransform.position);
            meleeAttackDestinationSet = false;
            agent.isStopped = true;
            if (Time.time > nextMeleeAttackTime)
            {
                MeleeHit(currentSelectedMeleeweapon.damageType, currentSelectedMeleeweapon.damage);
                nextMeleeAttackTime = Time.time + (currentSelectedMeleeweapon.attackSpeed/100) * (1 + Random.Range(0f,10/meleeAttackSkill)); // next attack time is based on the weapon speed and on the meleeAttackSkill
            }
        }else if(!meleeAttackDestinationSet)
        {
            //Debug.Log("setting new target");
            base.SetDestination(currentAttackingTargetTransform.position);
            meleeAttackDestinationSet = true;
            agent.isStopped = false;

        }
    }

    private void MeleeHit(DamageType damageType, int damage) //waiting for Update //TODO add someNice Functions here
    {
        //Debug.Log("I attacked Melee"); //later damage will be based on skill
        currentAttackingTarget.HandleAttack(damageType, damage);
        animator.SetTrigger("Attack");
        Debug.Log("Attack");
    }
        
    public override void HandleAttack(DamageType damageType, int damageAmount)
    {
        //Defense option //normalerweise 5 % chance zu blocken, defense Skill erhöht diese chance
        if (Random.Range(0f, 100f) > 95 - meleeDefenseSkill / 1.2f)
        {
            //defend, play defend animation
            Debug.Log("Defend");
            animator.SetTrigger("Defend");
        }
        else { 
            GetDamage(damageType, damageAmount);
            Debug.Log("getDamage");
            //animator.SetTrigger("getDamage");
        }
    }

  
       
                                                                                         //meleeWeapon type is class extending meleeWeapon which holds several meleeWeaponAttackTypess
   /* public void HandleAttack(DamageType damageType, int damageAmount, MeleeAttackDirection attackDirection, MeleeWeapon.MeleeWeaponType meleeWeaponType, MeleeWeaponAttackType meleeWeaponAttackType) 
    {
        //if we have a meleeeWeapon selected we can defend ourselves and maybe change the nextAttackTime of us or our enemy(like a counterattack) based on our melleAttack and Defense skill

        //after damage we get and ocassionaly the change in the nextAttackType of us our the attacker we will get a calculatet damage value, here we will change it based on armor
        float calculatedDamage; 
        
        int finalDamage = 0; //=calculatet Damage - armor or smth

        GetDamage(damageType,finalDamage);



    }*/


    #region MissileAttack & Aim
    void MissileAttack()
    {
        //make aiming thingis every x Seconds, we automaticly stop if we are not attacking anymore
        if (Time.time > nextMissileAttackTime)  //for performance optimisation- this gets called once per x frames, but also not at the same time by everone
        {
            PrepareMissileAttack();
            //Debug.Log("prepareMissileAttack");
            nextMissileAttackTime = Time.time + missileAttackIntervall;
        }

        //set Weapon rotation
        currentSelectedMissileWeapon.transform.localRotation = Quaternion.RotateTowards(currentSelectedMissileWeapon.transform.localRotation, wishedWeaponRotation, currentSelectedMissileWeapon.aimSpeed);
        //turn to target + predicted offset
        base.TurnToDestination(currentAttackingTargetTransform.position + currentAttackingTarget.agent.velocity.normalized * predictedAttackingPositionOffset);

        // PrepareMissileAttack returns MissileAttackPrepared = true, always
        if (missileAttackPrepared) //we need to send it to false when we are not attacking the target anymore but when?
        {
            //checken ob wir gezielt haben, dann Schuss
            if (HasAimed() && currentSelectedMissileWeapon.weaponReadyToShoot)
            {
                if (currentSelectedMissileWeapon.AmmoLeft())
                {
                    RandomRotator(currentSelectedMissileWeapon);
                    currentSelectedMissileWeapon.Shoot();
                    raycastSendForThisAttack = false;
                }
                //else Debug.Log("no Ammo left");
            }
            else missileAttackPrepared = false;
        }
    }

    private bool HasAimed()
    {
        bool hasAimed = false;

        //check if our weapon is angled properly
        //if () hasAimed =  true;

        //check if we have rotated to the enemy
        if (Quaternion.Angle(transform.rotation, wishRotation) < 5 && weapons[selectedWeapon].transform.localRotation == wishedWeaponRotation)  //hier kommt ein anderer Drehcode, weil er sonst die letzten grad vie lzu langsam dreht
        {
            agent.updateRotation = false;
            Vector3 ourPosition = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 destinationPosition = new Vector3(currentAttackingTargetTransform.position.x + currentAttackingTarget.agent.velocity.normalized.x * predictedAttackingPositionOffset, 0f, currentAttackingTargetTransform.position.z + currentAttackingTarget.agent.velocity.normalized.z * predictedAttackingPositionOffset);
            Quaternion perfectAimRotation = Quaternion.LookRotation(destinationPosition - ourPosition);

            transform.rotation = perfectAimRotation;
            agent.updateRotation = true;
            hasAimed = true;
        }

        return hasAimed;
        
    }

 
    public void  PrepareMissileAttack()
    {
        //if (currentAttackingTargetTransform != null) { 
            MissileWeapon weapon = weapons[selectedWeapon] as MissileWeapon; //cast Notwendig //später das nur einmal machen beim selectWeapon

            //wenn laudable - dann lade hier falls nicht geladen ist, wir laden schon bevor wir in Range sind
            if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable && !weapon.weaponReadyToShoot && !weapon.isPreparingWeapon)
            {
                TurnToDestination(currentAttackingTargetTransform.position);
                StartCoroutine("LoadWeapon");
                //Loading
            }

       
            if (Vector3.Distance(transform.position, currentAttackingTargetTransform.position) < weapon.missileRange) // wenn wir im Range sind
            {
                agent.isStopped = true;
                if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Loadable) //extraabfrage loadable Weapons können nicht beim laden zielen, bogen schon
                {
                    if (weapon.weaponReadyToShoot)
                    {
                        Aim(weapon);
                        //Debug.Log("aim aufgerufen");
                    }
                    else
                    {
                        TurnToDestination(currentAttackingTargetTransform.position);
                    }
                }
                else
                {
                    Aim(weapon);
                    //Debug.Log("aim aufgerufen");
                }//sonst drawable Weapons können während des drawen zielen


                if (Quaternion.Angle(transform.rotation, wishRotation) < 5 && !raycastSendForThisAttack)
                {//if turnedToDestination
                    if (automaticDirectFire) directFire = DirectFireCheck(weapon);
                    raycastSendForThisAttack = true;
                    //Debug.Log("send Raycast");
                }

                if (weapon.missileWeaponType == MissileWeapon.MissileWeaponType.Drawable) //den bogen/Wurfspeer spannen wir erst wenn wir in range sind
                {
                    if (!weapon.weaponReadyToShoot && !weapon.isPreparingWeapon) StartCoroutine("WeaponSpannen");
                }



                missileAttackPrepared = true; //only in range
            }
            else
            {
            //Wenn nicht im Range
            SetDestinationAttack(currentAttackingTargetTransform.position);   
            //hasCheckedDirectFire = false;
            missileAttackPrepared = false; //only in range
            }
        //}

    }

   /* private void FollowEnemyIntoHisRange() //TODO another time or just if we are on a wall, we have a wall mode where our agent only moves on the wall
    {
        //is a point from me times the target agents velocitxy on a navmesh?
        if(justWentOutOfRange) followingVector = currentAttackingTarget.agent.velocity;
        Vector3 pointInRange = transform.position + followingVector;

        SetDestinationAttack(currentAttackingTargetTransform.position);
        float remainingDistance = agent.remainingDistance;
        //Debug.Log(remainingDistance);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pointInRange, out hit, 1f, NavMesh.AllAreas))
        {
            SetDestinationAttack(pointInRange);
            //Debug.Log("new " + agent.remainingDistance);
            if(agent.remainingDistance>remainingDistance) SetDestinationAttack(currentAttackingTargetTransform.position);
        }
        //is this pouint in range? and is the path to his shorter than to the target agent?
        //if yes set this point as new setination
    }*/

    bool DirectFireCheck(MissileWeapon weapon) //returns true if directFire is checked
    {
        bool isTheWayFree=false;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 direction = currentAttackingTargetTransform.position - weapon.transform.position;
        direction.Normalize();
        if (Physics.Raycast(weapon.transform.position + direction, direction, out hit, weapon.missileRange)) //hier mal layermask zur performanceOptimierung hinzufügen
        {
            //Debug.Log(hit.collider.gameObject);
            if (hit.collider.gameObject.GetComponent<UnitMovement>() == currentAttackingTarget) isTheWayFree = true;
            else isTheWayFree = false;
        }
        return isTheWayFree;
    }

    void Aim(MissileWeapon weapon) //returns true if aiming is finished -
    {
        bool predictedFutureLocation = false;
        //Debug.Log("Aim");
    //goto here
    AimAtPredicted:


        Vector3 distDelta = currentAttackingTargetTransform.position - weapon.launchPoint.transform.position;
        float launchAngle = GetLaunchAngle(
            weapon.missileLaunchVelocity,
            new Vector3(distDelta.x, 0f, distDelta.z).magnitude,          //Vector3.Distance(new Vector3(currentAttackingTargetTransform.x, 0f, currentAttackingTargetTransform.z), new Vector3(transform.position.x, 0f, transform.position.z)),
            distDelta.y,                                                  //currentAttackingTargetTransform.y - transform.position.y,
            directFire
        );
        //Debug.Log(launchAngle);

        if (float.IsNaN(launchAngle))
        {
            //Debug.Log("Too far from Target - NaN");
            launchAngle = 0;
            //TODO dont shoot anymore, get nearer to target, but should not happen
        }



        //new time in air https://www.youtube.com/watch?v=jb2dWXp_tlw&t=234s&list=LLnkuTCY2XUW7UV3g2Apo5ww&index=2
        //initiallVelocityYComponent = missileLaunchVelocity * sin(launachAngle)
        //initiallVelocityYComponent = vY
        //gravity = g in positive magnitude
        //Formula: time in air = (vY + Sqr[(vY)²-4*(0.5*g)*(-(startH-finalH))]/g
        // but this works only for destinations lower than our starting keight, maybe make a abs of (startH-finalH)
        if (!predictedFutureLocation)
        {
            float timeInAir;
            float g = Physics.gravity.magnitude;
            float vY = weapon.missileLaunchVelocity * Mathf.Sin(launchAngle * (Mathf.PI / 180));
            //vY = 5f;
            float startH = weapon.launchPoint.transform.position.y;
            float finalH = currentAttackingTargetTransform.position.y;

            if (finalH < startH) {
                timeInAir = (vY + Mathf.Sqrt((float)(Mathf.Pow(vY, 2) - 4 * (0.5 * g) * (-(startH - finalH))))) / g;
            } else
            {
                //t = distanceX/initiallVeclocityXComponent
                float vX = weapon.missileLaunchVelocity * Mathf.Cos(launchAngle * (Mathf.PI / 180));
                float distanceX = Vector3.Distance(currentAttackingTargetTransform.position, weapon.launchPoint.transform.position);
                timeInAir = distanceX / vX;
            }

            //change the currentAttackingTargetTransform based on this time , take his velocity times this time  //predict his future location
       
                predictedAttackingPositionOffset =  currentAttackingTarget.agent.velocity.magnitude * (timeInAir);
                predictedFutureLocation = true;
                goto AimAtPredicted;
        }
        else
        {
           // Debug.Log("launchAngle: " + launchAngle);
            SetWishedWeaponRotation(launchAngle, weapon);  //wenn wir zuende mir der Waffe gezielt haben
        }
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
    
    private void SetWishedWeaponRotation(float launchAngle,MissileWeapon weapon)
    {
        launchAngle = -launchAngle; //cause localTransform goes in the other direction
        float yTilt = 0f; ; //we when the back is on the side of the Units it also needs to aim directly at the enemy - trigonometrie cos(a) = b/c, now theres a etter way
       
        Quaternion angleOfWeapon = Quaternion.LookRotation((currentAttackingTargetTransform.position - weapon.transform.position));
        Quaternion angleOfUnit = Quaternion.LookRotation((currentAttackingTargetTransform.position - transform.position));
        yTilt = Quaternion.Angle(angleOfWeapon, angleOfUnit);
       
        wishedWeaponRotation = Quaternion.Euler(transform.rotation.eulerAngles.x  + launchAngle, transform.rotation.eulerAngles.x - yTilt, transform.rotation.eulerAngles.z);
        
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
            weapon.transform.Rotate(
                Random.Range(-step, step),
                Random.Range(-step, step),
                Random.Range(-step, step));

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

    #endregion


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
        //Debug.Log("aborted Attack");
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


   


}


