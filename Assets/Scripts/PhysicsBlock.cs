using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBlock : MonoBehaviour
{
    // For when the shape isn't a box or a sphere
    [SerializeField] public float massMultiplier = 1;
    [SerializeField] public BlockMaterial material;

    public virtual bool IsFragile => false;

    public virtual void HitBlock(Vector3 relativeVelocity, float mass)
    {

    }

    // To be run in edit mode
    public void ApplyMaterial(bool changeVisuals)
    {
        // Set the visual material
        if (changeVisuals)
        {
            Material mat = material.GetRandomMat();

            if (mat != null)
                GetComponent<MeshRenderer>().material = mat;
        }

        // Calculate the mass
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        if (boxCollider == null && meshCollider == null & sphereCollider == null)
            throw new System.Exception("No collider found!");

        float volume = 0;

        if (boxCollider != null)
        {
            volume = boxCollider.size.x * boxCollider.size.y * boxCollider.size.z;
        }

        if (meshCollider != null)
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            volume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z * massMultiplier;
        }

        if(sphereCollider != null)
        {   
            float radiusCubed = sphereCollider.radius * sphereCollider.radius * sphereCollider.radius;
            volume = (4f / 3f) * Mathf.PI * radiusCubed;
        }

        if (volume < 0.0001f)
            throw new System.Exception("Could not calculate volume!");

        GetComponent<Rigidbody>().mass = volume * material.density;
    }
}
