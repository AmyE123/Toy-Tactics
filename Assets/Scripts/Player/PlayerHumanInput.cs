using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHumanInput : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] GameSettings _settings;

    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    public void DoUpdate()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            _player.Cameras.ToggleCameraType();
        }

        if (_player.Cameras.IsThirdPerson)
            UpdateThirdPersonControls();
        
        if (_player.Cameras.IsFirstPerson)
            UpdateFirstPersonControls();
    }

    void UpdateFirstPersonControls()
    {
        Vector2 playerInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerInput = Vector2.ClampMagnitude(playerInput, 1);

        _player.Movement.move.desiredVelocity = ((playerInput.x * transform.right) + (playerInput.y * transform.forward)) * _player.Movement.move.maxSpeed;
		_player.Movement.jump.isRequested |= Input.GetButtonDown("Jump");

        if (_player.Equipment.IsMovementBlocked)
        {
            _player.Movement.move.desiredVelocity = Vector3.zero;
            _player.Movement.jump.isRequested = false;
        }

        float rotX = Input.GetAxis("Mouse X") * _settings.mouseSensitivity;
        float rotY = Input.GetAxis("Mouse Y") * _settings.mouseSensitivity;

        bool isWeaponActive = Input.GetKey(KeyCode.E) || Input.GetMouseButton(1);

        if (isWeaponActive)
        {
            rotX = 0;
            rotY = 0;
        }

        if (_settings.invertX)
            rotX = -rotX;

        if (_settings.invertY)
            rotY = -rotY;

        float currentY = _player.Cameras.firstPersonCam.transform.localEulerAngles.x;

        if (currentY > 180)
            currentY -= 360;

        float newY = Mathf.Clamp(currentY - rotY, -90, 90);

        transform.Rotate(new Vector3(0, rotX, 0));
        _player.Cameras.firstPersonCam.transform.localEulerAngles = new Vector3(newY, 0, 0);
    }

    void UpdateThirdPersonControls()
    {
        Vector2 playerInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerInput = Vector2.ClampMagnitude(playerInput, 1);

        Vector3 rightDir = _cam.transform.right;
        Vector3 forwardDir = _cam.transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();

        _player.Movement.move.desiredVelocity = ((playerInput.x * rightDir) + (playerInput.y * forwardDir)) * _player.Movement.move.maxSpeed;
		_player.Movement.jump.isRequested |= Input.GetButtonDown("Jump");
        _player.Movement.setFacing = false;

        if (_player.Equipment.IsMovementBlocked)
        {
            _player.Movement.move.desiredVelocity = Vector3.zero;
            _player.Movement.jump.isRequested = false;
        }

        if (_player.Movement.jetpackActive)
            _player.Movement.jump.isRequested = false;

        if (_player.Movement.move.desiredVelocity.magnitude > 0.25f)
        {
            bool isJetpacking = _player.Movement.jetpackActive && _player.Movement.IsGrounded == false;

            if (isJetpacking == false)
                _player.Animations.SetAsRunning();
    
            _player.Movement.targetRot = Quaternion.LookRotation(_player.Movement.move.desiredVelocity, Vector3.up);
            _player.Movement.setFacing = true;
        }
    }

}
