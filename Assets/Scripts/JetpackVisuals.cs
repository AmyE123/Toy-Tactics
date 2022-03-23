using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackVisuals : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _mesh;

    [SerializeField]
    private ParticleSystem[] _smokeParticles;

    private bool _reallyActive;

    public void SetJetpackActive(bool activeState, bool reallyActive)
    {
        gameObject.SetActive(activeState);
        _reallyActive = reallyActive;
    }

    public void UpdateParticles()
    {
        bool activeState = gameObject.activeSelf && Input.GetKey(KeyCode.Space) && _reallyActive;

        foreach (ParticleSystem parts in _smokeParticles)
        {
            var emission = parts.emission;
            emission.enabled = activeState;
        }
    }

    public void SetMaterial(Material mat)
    {
        _mesh.material = mat;
    }
}
