using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _camDistance;
    [SerializeField] private float _camHeight;
    [SerializeField] private float _camLerpSpeed;
    [SerializeField] private GameSettings _settings;
    [SerializeField] private LayerMask _everythingButPlayer;

    // Update is called once per frame
    void Update()
    {
        bool isWeaponActive = Input.GetKey(KeyCode.E) || Input.GetMouseButton(1);

        FollowPosition();

        if (isWeaponActive == false)
            InputRotation();

        RaycastDistance();
    }

    private void FollowPosition()
    {
        transform.position = _objectToFollow.transform.position;
    }

    private void InputRotation()
    {
        float rotX = Input.GetAxis("Mouse X") * _settings.MouseSensitivity;
        float rotY = Input.GetAxis("Mouse Y") * _settings.MouseSensitivity;

        if (_settings.InvertX)
            rotX = -rotX;

        if (_settings.InvertY)
            rotY = -rotY;

        float currentUpDown = transform.localEulerAngles.x;

        if (currentUpDown > 180)
            currentUpDown -= 360;

        float newLeftRight = transform.localEulerAngles.y + rotX;

        float newUpDown = Mathf.Clamp(currentUpDown - rotY, -90, 90);

        transform.localEulerAngles = new Vector3(newUpDown, newLeftRight, 0);
    }

    private void RaycastDistance()
    {
        Vector3 targetPos = new Vector3(0, _camHeight, -_camDistance);

        Vector3 dirToCam = (_camTransform.position - _objectToFollow.transform.position).normalized;
        if (Physics.Raycast(_objectToFollow.transform.position, dirToCam, out RaycastHit hit, _camDistance, _everythingButPlayer))
        {
            //TODO: Still not sure what this is. Make this function more clear.
            targetPos = new Vector3(0, _camHeight, 0.1f-hit.distance);
        }

        _camTransform.localPosition = Vector3.Lerp(_camTransform.localPosition, targetPos, Time.deltaTime * _camLerpSpeed);
    }
}
