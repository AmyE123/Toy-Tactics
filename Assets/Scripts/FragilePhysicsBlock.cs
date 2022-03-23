using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePhysicsBlock : PhysicsBlock
{
    public override bool IsFragile => true;

    private Rigidbody rb;

    private Rigidbody RB
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            return rb;
        }
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.magnitude > material.breakVelocity)
        {
            Smash();
        }
    }

    public override void HitBlock(Vector3 relativeVelocity, float mass)
    {
        float force = relativeVelocity.magnitude * mass;

        if (force > material.breakForce)
            Smash();
    }

    public void DoExplosiveDamage(Vector3 position, float radius, float force)
    {
        if (material.doesSmash == false)
            return;

        float distance = Vector3.Distance(transform.position, position);
        float distancePercent = Mathf.Sqrt(Mathf.Clamp01(distance / radius));

        float damageVal = Mathf.Sqrt(((1 - distancePercent) * force));

        if (damageVal > material.breakForce)
            Smash();
    }

    public void Smash()
    {
        Destroy(gameObject);       
    }
}