using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProperties : MonoBehaviour
{
    [SerializeField] int damage = 10; 
    [SerializeField] int bulletForce = 1000; 
    [SerializeField] float bulletLifetime = 5f;
    [SerializeField] bool destroyOnHit;
    [SerializeField] Rigidbody rb;

    public int BulletForce
    {
        get{ return bulletForce; }
        set{ bulletForce = value; }
    }

    public float BulletLifetime
    {
        get{ return bulletLifetime; }
        set{ bulletLifetime = value; }
    }

    public void Init(int dmg, float velocity)
    {
        Destroy(gameObject, bulletLifetime);
        
        damage = dmg;
        rb.velocity = transform.forward * velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            var playerProperties = col.gameObject.GetComponent<Player>();
            playerProperties.HealthValueAdjust(-damage, col);            
        }
        else if (col.gameObject.tag == "Battery")
        {
            var bat = col.gameObject.GetComponent<Battery>();
            bat.Damage(damage);
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
