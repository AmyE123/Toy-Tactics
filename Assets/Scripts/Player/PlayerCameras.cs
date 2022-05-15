using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameras : MonoBehaviour
{
    private const int CAM_AIM_SPEED = 5;

    public enum CameraView { ThirdPerson, FirstPerson, EnemyFollow };

    [SerializeField] CameraView _selectedCamera;

    [Header("Camera Properties")] 
    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera firstPersonCam;
    [SerializeField] ThirdPersonCamera _thirdPersonScript;

    public bool IsFirstPerson => _selectedCamera == CameraView.FirstPerson;

    public bool IsThirdPerson => _selectedCamera == CameraView.ThirdPerson;

    public void ToggleCameraType()
    {
        if (_selectedCamera == CameraView.FirstPerson)
            SetCameraThirdPerson();
        else
            SetCameraFirstPerson();       
    }
    
    public void SetCameraFirstPerson()
    {        
        _selectedCamera = CameraView.FirstPerson;
        thirdPersonCam.enabled = false;
        _thirdPersonScript.enabled = false;
        firstPersonCam.enabled = true; 
    }

    public void SetCameraThirdPerson()
    {
        _selectedCamera = CameraView.ThirdPerson;
        thirdPersonCam.enabled = true;
        _thirdPersonScript.enabled = true;
        firstPersonCam.enabled = false; 
    }

    public void DisableAllCameras()
    {
        thirdPersonCam.enabled = false;
        firstPersonCam.enabled = false;
        _thirdPersonScript.enabled = false;
    }

    public bool AimFirstPersonCamera(Vector3 aimTarget)
    {
        Vector3 vectorToTarget = aimTarget - transform.position;
        vectorToTarget.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        firstPersonCam.transform.rotation = Quaternion.Lerp(firstPersonCam.transform.rotation, targetRotation, Time.deltaTime * CAM_AIM_SPEED);
        
        //TODO: Make more clear what this does
        return Vector3.Dot(vectorToTarget, firstPersonCam.transform.forward) > 0.9995f;
    }
}
