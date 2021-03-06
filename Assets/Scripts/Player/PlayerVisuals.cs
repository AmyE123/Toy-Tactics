using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private const int FIRST_PERSON_ALPHA_TIME = 2;
    private const int ALPHA_TIME = 5;

    [SerializeField] private Rigidbody[] _gibs;
    [SerializeField] private JetpackVisuals _jetpack;
    [SerializeField] private Transform _heavyBoots;
    [SerializeField] private Material _playerMat;
    [SerializeField] private Player _player;
    [SerializeField] private float _firstPersonAlpha;
    
    private float materialAlpha = 1f;
    private Material _myPersonalMaterial;
    
    void Update()
    {
        ManageMaterialAlpha();
        _jetpack.UpdateParticles();
    }

    void OnDisable()
    {
        _myPersonalMaterial.SetFloat("_Opacity", 1);        
    }

    public void SetBootsActive(bool state) => _heavyBoots.gameObject.SetActive(state);
    
    public void SetJetpackActive(bool visibleState, bool actualState)
    {
        _jetpack.SetJetpackActive(visibleState, actualState);
    }

    void ManageMaterialAlpha()
    {
        _myPersonalMaterial.SetFloat("_Opacity", materialAlpha);

        if(_player.Cameras.IsFirstPerson)
        {
            materialAlpha = Mathf.Lerp(materialAlpha, _firstPersonAlpha, FIRST_PERSON_ALPHA_TIME * Time.deltaTime);    
            if(_player.Status == Player.PlayerStatus.Idle)
            {
                materialAlpha = 1;
            }
        }
        else
        {
            materialAlpha = Mathf.Lerp(materialAlpha, 1, ALPHA_TIME * Time.deltaTime);
        }
    }

    public void SpawnGibs()
    {
        foreach (Rigidbody rb in _gibs)
        {
            rb.gameObject.SetActive(true);
            rb.transform.SetParent(null);
            rb.GetComponent<MeshRenderer>().material = _myPersonalMaterial;
        }
    }

    public void SpawnGibsWithForce(Vector3 pos, float force, float radius)
    {
        foreach (Rigidbody rb in _gibs)
        {
            rb.gameObject.SetActive(true);
            rb.transform.SetParent(null);
            rb.GetComponent<MeshRenderer>().material = _myPersonalMaterial;
            rb.AddExplosionForce(force, pos, radius);
        }
    }

    public void SetPlayerMaterial(TeamData teamData)
    {
        if (teamData == null)
        {
            return;
        }

        Material mat = new Material(_playerMat);
        mat.color = teamData.color;
        mat.SetColor("_AlbedoColor", teamData.color);

        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            rend.material = mat;

        foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (rend.tag != "KeepMaterial")
                rend.material = mat;
        }

        _myPersonalMaterial = mat;
        _jetpack.SetMaterial(mat);
    }
}
