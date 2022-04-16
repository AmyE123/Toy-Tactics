using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Explosive
{
    [SerializeField] int _damageRequiredToExplode;
    [SerializeField] int _explosiveDamageRequiredToExplode;

    public void Damage(int damagePoints)
    {
        if (damagePoints >= _damageRequiredToExplode)
            LightFuse();
    }

    public void DoExplosiveDamage(Vector3 position, float radius, float force)
    {
        float distance = Vector3.Distance(transform.position, position);
        float distancePercent = Mathf.Sqrt(Mathf.Clamp01(distance / radius));

        float damageVal = Mathf.Sqrt(((1 - distancePercent) * force));

        if (damageVal > _explosiveDamageRequiredToExplode)
        {
            ReduceFuseTime(Random.Range(0.5f, 0.8f));
            LightFuse();
        }
    }

}
