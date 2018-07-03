using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    int damage;
    DamageType damageType;
    [Tooltip("a bullet is a ball, but a bold or arrow are not")]
    public bool ball = false;

    public Rigidbody rb;

    private void Start()
    {
            //for better performance, dont do this every frame
            InvokeRepeating("AdjustRotation", 0f, 0.2f);
  
    }

    public void setDamage(int damage, DamageType damageType)
    {
        this.damage = damage;
        this.damageType = damageType;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Projectile") { 
            if (collision.gameObject.tag == "Unit")
            {
                collision.gameObject.GetComponent<UnitMovement>().GetDamage(damageType,damage);
            }

            Destroy(gameObject.GetComponent<Rigidbody>()); //so our projectiles will stick - for now
            if(!collision.gameObject.isStatic) transform.parent = collision.gameObject.transform; //cause some static objects like ground etc are scaled wrong
            Destroy(gameObject,5);
        }

    }

    void AdjustRotation()
    {
        if (!ball && rb != null)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
}
