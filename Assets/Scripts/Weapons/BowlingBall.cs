using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : EquipableItem
{
    bool _hasUsed = false;

    [SerializeField] private float _throwVelocity;
    [SerializeField] private Transform _throwPoint;
    [SerializeField] private Transform _previewObj;
    [SerializeField] private GameObject _projectilePrefab;

    public override bool ShouldStopMovement => _hasUsed;

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

    public override void StartUse()
    {
        base.StartUse();
        
        if (_hasUsed)
            return;
        
        _previewObj.gameObject.SetActive(false);
        _hasUsed = true;

        Camera cam = Camera.main;
        Vector3 distancePoint = cam.transform.position + (cam.transform.forward * 100);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 100))
        {
            distancePoint = hit.point;
        }

        GameObject newObj = Instantiate(_projectilePrefab, _throwPoint.position, _throwPoint.rotation);
        newObj.transform.LookAt(distancePoint);
        newObj.GetComponent<Rigidbody>().velocity = newObj.transform.forward * _throwVelocity;

        EndTurnInSeconds(3f);
    }
}
