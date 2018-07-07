using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    int damageBonus;
    DamageType damageType;
    [Tooltip("a bullet is a ball, but a bold or arrow are not")]
    public bool ball = false;
    bool isFlying;

    public Rigidbody rb;

    private void Start()
    {
            //for better performance, dont do this every frame
            InvokeRepeating("AdjustRotation", 0f, 0.2f);
            
    }

    public void SetFlyingParams(int damage, DamageType damageType, Vector3 velocity)
    {
        this.damageBonus = damage;
        this.damageType = damageType;
        rb.velocity = velocity;
        rb.isKinematic = false;
        isFlying = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Projectile") { 
            if (collision.gameObject.tag == "Unit")
            {
                collision.gameObject.GetComponent<UnitMovement>().GetDamage(damageType,damageBonus);
            }

            //Destroy(gameObject.GetComponent<Rigidbody>()); //so our projectiles will stick - for now
            rb.isKinematic = true;
            //if(!collision.gameObject.isStatic) transform.parent = collision.gameObject.transform; //cause some static objects like ground etc are scaled wrong
            //parenten auslassen, anderen Stickcode ausdenken
            //Destroy(gameObject,5);
            isFlying = false;
        }

    }

    void AdjustRotation()
    {
        if (!ball && isFlying)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
}
