using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockMaterial : ScriptableObject
{
    public Material[] materials;
    public float density;
    public bool doesSmash;
    public float breakForce;
    public float breakVelocity;

    public Material GetRandomMat()
    {
        if (materials.Length == 0)
            return null;

        int i = Random.Range(0, materials.Length);
        return materials[i];
    }
}
