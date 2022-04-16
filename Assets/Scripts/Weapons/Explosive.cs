using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosive : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] float _force;
    [SerializeField] float _fuseTime;
    [SerializeField] bool _isFuseLit;
    [SerializeField] GameObject _explodeParticles;
    [SerializeField] AudioClip _explodeSound;
    [SerializeField] float _explodePitchMin = 0.9f;
    [SerializeField] float _explodePitchMax = 1.1f;
    [SerializeField] float _damageMultiplier = 1f;

    bool _hasExploded = false;

    protected void ReduceFuseTime(float mul)
    {
        _fuseTime *= mul;
    }

    public float LightFuse()
    {
        transform.DOShakePosition(1.0f, 0.15f, 100).SetDelay(_fuseTime - 0.5f);
        transform.DOScale(transform.localScale * 1.4f, 0.2f).SetDelay(_fuseTime - 0.2f).SetEase(Ease.InExpo);
        _isFuseLit = true;
        return _fuseTime;
    }

    public virtual void Update()
    {
        if (_isFuseLit == false)
            return;

        _fuseTime -= Time.deltaTime;
        
        if (_fuseTime <= 0)
            Explode();
    }

    private void Explode()
    {
        if (_hasExploded)
            return;

        _hasExploded = true;
        SoundManager.Instance.PlaySoundWithRandomPitch(_explodeSound, transform.position, 1, _explodePitchMin, _explodePitchMax);

        foreach(Collider col in Physics.OverlapSphere(transform.position, _radius))
        {
            if (col.gameObject == gameObject)
                continue;

            col.attachedRigidbody?.AddExplosionForce(_force, transform.position, _radius);

            if (col.tag == "Player")
            {
                col.GetComponent<Player>().DoExplosiveDamage(transform.position, _radius, _force, _damageMultiplier);
            }
            else if (col.gameObject.tag == "Battery")
            {
                var bat = col.gameObject.GetComponent<Battery>();
                bat.DoExplosiveDamage(transform.position, _radius, _force);
            }
            else
            {
                // Todo maybe use tags for fragile glass too?
                var block = col.GetComponent<FragilePhysicsBlock>();
                block?.DoExplosiveDamage(transform.position, _radius, _force);
            }
        }
        
        if (_explodeParticles != null)
        {
            GameObject newParticles = Instantiate(_explodeParticles, transform.position, Quaternion.identity);
            Destroy(newParticles, 8);
        }

        DOTween.Kill(transform);
        Destroy(gameObject);
    }
}
