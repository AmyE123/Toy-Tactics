using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : EquipableItem
{
    [SerializeField] int _shotCount;
    [SerializeField] int _bulletsPerShot;
    [SerializeField] float _timeToShoot;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _shootTransform;

    [Header("Shot information")]
    [SerializeField] int _shotMinDamage;
    [SerializeField] int _shotMaxDamage;
    [SerializeField] float _shotSpeed;
    [SerializeField] float _angleSpread;
    [SerializeField] AudioClip _shotSound;

    bool _hasUsed;
    bool _isShooting;
    float _shootTimeRemaining;

    public override bool ShouldStopMovement => _isShooting;

    public override void StartUse()
    {
        base.StartUse();

        if (_hasUsed)
            return;
        
        _isShooting = true;
        _hasUsed = true;

        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        // TODO take into account bullet spread
        Camera cam = Camera.main;

        for (int i=0; i<_shotCount; i++)
        {
            Vector3 distancePoint = cam.transform.position + (cam.transform.forward * 100);

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 100))
            {
                distancePoint = hit.point;
            }

            for (int j=0; j<_bulletsPerShot; j++)
            {
                Vector3 dirVec = (distancePoint - transform.position).normalized;
                Vector3 angle = Quaternion.LookRotation(dirVec, Vector3.up).eulerAngles;

                angle.x += Random.Range(-_angleSpread, _angleSpread);
                angle.y += Random.Range(-_angleSpread, _angleSpread);
                angle.z += Random.Range(-_angleSpread, _angleSpread);

                var bulletFire = Instantiate(_bulletPrefab, _shootTransform.position, _shootTransform.rotation);
                bulletFire.transform.rotation = Quaternion.Euler(angle);
                SoundManager.Instance.PlaySoundWithRandomPitch(_shotSound, _shootTransform.position, 1, 0.8f, 1.2f);

                int dmg = Random.Range(_shotMinDamage, _shotMaxDamage + 1);
                bulletFire.GetComponent<BulletProperties>().Init(dmg, _shotSpeed);  
            }

            yield return new WaitForSeconds(_timeToShoot / _shotCount);
        }

        EndTurnInSeconds(1.5f);
    }

    public void FinishUse()
    {

    }

    public override void OnEquip()
    {
        base.OnEquip();
        FindObjectOfType<UIManager>().ShowCrosshair();
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        FindObjectOfType<UIManager>().HideCrosshair();
    }
}
