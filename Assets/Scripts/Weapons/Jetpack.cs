using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : EquipableItem
{
    [SerializeField]
    private float _fuel = 100;

    [SerializeField]
    private float _maxFuel = 100;

    [SerializeField]
    private float _fuelBurnSpeed = 10;
    
    [SerializeField]
    private float _upwardsThrust = 100;

    [SerializeField]
    private float _maxMoveSpeed;

    [SerializeField]
    private float _maxAcceleration;

    [SerializeField]
    private float _maxVerticalAcceleration;

    [SerializeField]
    private Vector3 _desiredVelocity;

    bool _hasBeenUsed;
    bool _isActive;
    Camera _mainCam;

    public override bool ShouldStopMovement => _isActive && Input.GetKey(KeyCode.Space);
    
    public float FuelPercent => Mathf.Clamp01((float) _fuel / (float) _maxFuel);

    public void Awake()
    {
        _mainCam = Camera.main;
    }

    public override void StartUse()
    {
        base.StartUse();

        if (_hasBeenUsed)
            return;

        _isActive = true;
        _hasBeenUsed = true;
        FindObjectOfType<JetpackUI>().SetVisible(this);
        _player.SetJetpackActive(true, true);
    }

    public void FinishUse()
    {
        _isActive = false;
        _player.SetJetpackActive(true, false);
    }

    public override void OnEquip()
    {
        base.OnEquip();
        _player.SetJetpackActive(true, false);
        _fuel = _maxFuel;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        _player.SetJetpackActive(false, false);
    }

    void Update()
    {
        GetPlayerInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isActive && Input.GetKey(KeyCode.Space) && _fuel > 0)
        {
            _fuel -= Time.deltaTime * _fuelBurnSpeed;
            AdjustVelocity();

            if (_fuel <= 0)
                FinishUse();
        }
    }

    void GetPlayerInput()
    {
        Vector2 playerInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerInput = Vector2.ClampMagnitude(playerInput, 1);

        float upwardBoost = Input.GetKey(KeyCode.Space) ? _upwardsThrust : 0;

        Vector3 forward = _mainCam.transform.forward;
        Vector3 right = _mainCam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        _desiredVelocity = ((playerInput.x * right)
                          + (playerInput.y * forward)
                          + (upwardBoost * Vector3.up)) * _maxMoveSpeed;
    }

    void AdjustVelocity () 
    {
        Vector3 velocity = _playerRigidBody.velocity;

		float maxSpeedChange = _maxAcceleration * Time.deltaTime;
		float maxVSpeedChange = _maxVerticalAcceleration * Time.deltaTime;

		velocity.x = Mathf.MoveTowards(velocity.x, _desiredVelocity.x, maxSpeedChange);
		velocity.y = Mathf.MoveTowards(velocity.y, _desiredVelocity.y, maxVSpeedChange);
		velocity.z = Mathf.MoveTowards(velocity.z, _desiredVelocity.z, maxSpeedChange);

        _playerRigidBody.velocity = velocity;
    }
}
