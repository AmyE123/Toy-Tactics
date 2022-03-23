using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEquipable : EquipableItem
{
    bool _hasUsed = false;
    [SerializeField] private LineRenderer _trajectoryLine;
    [SerializeField] private float _throwVelocity;
    [SerializeField] private Transform _throwPoint;
    [SerializeField] private Transform _previewObj;
    [SerializeField] private GameObject _bombPrefab;

    public override void OnEquip()
    {
        base.OnEquip();
        _trajectoryLine.enabled = true;
    }

    public override void StartUse()
    {
        base.StartUse();
        
        if (_hasUsed)
            return;
        
        _previewObj.gameObject.SetActive(false);
        _hasUsed = true;

        GameObject newObj = Instantiate(_bombPrefab, _throwPoint.position, _throwPoint.rotation);
        newObj.GetComponent<Rigidbody>().velocity = _throwPoint.forward * _throwVelocity;
        float fuseTime = newObj.GetComponent<Explosive>().LightFuse();
        
        EndTurnInSeconds(fuseTime + 1);
        _trajectoryLine.enabled = false;
        _player.Equipment.UnequipItem();
    }

    void Update()
    {
        if (_hasUsed == false)
        {
            List<Vector3> traj = DetermineTrajectory(_throwPoint.position, _throwPoint.forward);
            _trajectoryLine.positionCount = traj.Count;
            _trajectoryLine.SetPositions(traj.ToArray());
        }
    }

    List<Vector3> DetermineTrajectory(Vector3 startPosition, Vector3 direction)
    {
        List<Vector3> outPoints = new List<Vector3>();

        float maxDuration = 1.5f;
        float timeStepInterval = 0.05f;
        int maxSteps = (int) (maxDuration / timeStepInterval);

        float initialVel = _throwVelocity;

        for (int i=0; i<maxSteps; ++i)
        {
            Vector3 calculatedPos = startPosition + direction * initialVel * i * timeStepInterval;
            calculatedPos.y += Physics.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2);

            outPoints.Add(calculatedPos);
        }

        return outPoints;
    }

}
