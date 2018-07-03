using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    int damage;
    DamageType damageType;

	public void setDamage(int damage, DamageType damageType)
    {
        this.damage = damage;
        this.damageType = damageType;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            collision.gameObject.GetComponent<UnitMovement>().GetDamage(damageType,damage);
        }
        
        gameObject.GetComponent<Rigidbody>().isKinematic = true; //so our projectiles will stick - for now
        if(!collision.gameObject.isStatic) transform.parent = collision.gameObject.transform; //cause some static objects like ground etc are scaled wrong
        Destroy(gameObject,5);




    }
}
