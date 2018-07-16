using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnitProto : MeleeUnitProto {

    public GameObject projectilePrefab;
    public float missileRange;
    public float maxMissileVelocity; //könnte missileRange unnötig machen
    //private float currentMissileForce  = 100;
    private Rigidbody currentMissileRb;
    public int aimSkill;
    public float missileReloadTime = 1f;
    protected bool missileWeaponDrawn = true; 

    protected override void Update()
    {
        meleeWeaponDrawn = false;
        base.Update();
        if (state == State.Attacking && missileWeaponDrawn)
        {
            MissileAttackUpdate();
        }
    }

    public override void Attack(UnitMovement target)
    {
        Debug.Log("missileAttack");
        MissileAttack(target);
    }

    protected void MissileAttackUpdate()
    {

    }

    /*void MeleeAttack(UnitMovement target)
    {
        base.Attack(target);
    }*/

    void MissileAttack(UnitMovement target)
    {
        //instantiate projectile and than add force
        base.Attack(target);
        Aim(target);
        Instantiate(projectilePrefab, transform.position + transform.forward, transform.rotation);
        currentMissileRb.AddForce(transform.forward * maxMissileVelocity);
    }

    void Aim(UnitMovement target)
    {
        TurnToPosition(target.gameObject.transform.position);
        //SetDestination();
        //Aim igher
        //adjust force, set currentMissile FOrce
        //now it should perfectly hit, so we apply a skillbased random rotator function
    }

    //TODO Formel anwenden und Raycast - welcher sagt welcher winkel genommen wird
    float GetLaunchAngle(float speed,float distance,float heightDifference)
    {
        return 0f;
    }

    Vector3 GetAimVector(Vector3 start, Vector3 destination)
    {
        Vector3 distDelta = destination - start;
        float distance = new Vector3(distDelta.x, 0f, distDelta.z).magnitude;
        float heightDifference = distDelta.y;
        float launchAngle = GetLaunchAngle(maxMissileVelocity, distance, heightDifference);
        return transform.TransformDirection(new Vector3(0f, Mathf.Sin(launchAngle), Mathf.Cos(launchAngle)) * maxMissileVelocity);
        //wir haben den vektor wieviel nach oben - jetzt brauchen wir das in Wold coordinates
    }
}
