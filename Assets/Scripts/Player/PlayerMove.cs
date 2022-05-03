using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMove : MonoBehaviour
{
    public enum ControlMode { ThirdPerson, FirstPerson }

    [System.Serializable]
    public class JumpInfo
    {
        //TODO: Should these be a 'default' const?
        [Range(0f, 10f)] public float height = 2f;
        [Range(0f, 10f)] public float wallJumpPower = 2f;
        [Range(0f, 5f)] public int maxAirJumps = 0;
        public bool enableWallJump;

        [HideInInspector] public int phase;
        [HideInInspector] public int stepsSinceLastJump;
        [HideInInspector] public bool isRequested;

        //TODO: MAGIC NUMBER
        public float Speed => Mathf.Sqrt(-2f * Physics.gravity.y * height);
    }

    [System.Serializable]
    public class GroundInfo
    {
        //TODO: Should these be a 'default' const?
        [Range(0f, 90f)] public float maxSlopeAngle = 25f;
        [Range(0f, 100f)] public float maxSnapSpeed = 100f;
	    [Min(0f)] public float probeDistance = 1f;

        [HideInInspector] public int stepsSinceLastGrounded;
        [HideInInspector] public float minGroundDotProduct;

        [HideInInspector] public Vector3 contactNormal;
        [HideInInspector] public int groundContactCount;

        [HideInInspector] public Vector3 wallNormal;
        [HideInInspector] public int wallContactCount;

    }

    [System.Serializable]
    public class MoveInfo
    {
        //TODO: Should these be a 'default' const?
        [Range(0f, 100f)] public float maxAcceleration = 10f;
        [Range(0f, 100f)] public float maxAirAcceleration = 1f;
        [Range(0f, 100f)] public float maxSpeed = 10f;
        [Range(0f, 100f)] public float turnSpeed = 10f;
        
        [Range(0f, 100f)] public float maxMove = 10f;
        [Range(0f, 100f)] public float remaining = 10f;

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 desiredVelocity;
    }

    public JumpInfo jump;
    public GroundInfo ground;
	public MoveInfo move;
    public float gravityMultiplier;

    [SerializeField] GameSettings _settings;
    [SerializeField] SoldierAnimations _anims;
    [SerializeField] Player _player;
    
    Rigidbody _rb;
    Camera _cam;
    public bool setFacing;

    public Quaternion targetRot;
    public bool jetpackActive;

    public void SetJetpackActive(bool active) => jetpackActive = active;

	public bool IsGrounded => ground.groundContactCount > 0;

    bool OnWall => ground.wallContactCount > 0;

    public float MovePercent => Mathf.Clamp01(move.remaining / move.maxMove);

	void OnValidate () 
    {
		ground.minGroundDotProduct = Mathf.Cos(ground.maxSlopeAngle * Mathf.Deg2Rad);
	}

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        OnValidate();
        gravityMultiplier = 1;
    }

    void ClearState () 
    {
		ground.groundContactCount = 0;
		ground.contactNormal = Vector3.zero;
        ground.wallContactCount = 0;
        ground.wallNormal = Vector3.zero;
	}

    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        AdjustRotation();

        if (gravityMultiplier != 1)
        {
            Vector3 extraGravity = (gravityMultiplier - 1) * Physics.gravity;
            _rb.AddForce(extraGravity * _rb.mass);
        }

        if (jump.isRequested) 
			Jump();
        
        if ((jetpackActive && !IsGrounded) == false)
            _rb.velocity = move.velocity;
    
        ClearState();
    }

    void Jump()
    {
        if (jetpackActive)
            return;

        jump.isRequested = false;

        float jumpSpeed = jump.Speed;

        if (IsGrounded)
        {
            float alignedSpeed = Vector3.Dot(move.velocity, ground.contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
        
            move.velocity += ground.contactNormal  * jumpSpeed;
            _anims.StartJumping();
        }
        else if (OnWall && jump.enableWallJump)
        {
            Vector3 jumpDir = (ground.wallNormal + Vector3.up).normalized;
            move.velocity = jumpDir * jumpSpeed * jump.wallJumpPower;
        }
        else if (jump.phase <= jump.maxAirJumps)
        {
            move.velocity.y = jumpSpeed;
            _anims.StartJumping();
        }

        jump.stepsSinceLastJump = 0;
        jump.phase += 1;
    }

    Vector3 ProjectOnContactPlane (Vector3 vector) 
    {
		return vector - ground.contactNormal * Vector3.Dot(vector, ground.contactNormal);
	}

    void AdjustVelocity () 
    {
        if (jetpackActive && IsGrounded == false)
            return;

        if (move.remaining <= 0)
            move.desiredVelocity = Vector3.zero;

		Vector3 xAxis = ProjectOnContactPlane(Vector3.right);
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward);

        float currentX = Vector3.Dot(move.velocity, xAxis);
		float currentZ = Vector3.Dot(move.velocity, zAxis);

		float acceleration = IsGrounded ? move.maxAcceleration : move.maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX = Mathf.MoveTowards(currentX, move.desiredVelocity.x, maxSpeedChange);
		float newZ = Mathf.MoveTowards(currentZ, move.desiredVelocity.z, maxSpeedChange);

        move.velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        float amountMoved = Mathf.Min(move.velocity.magnitude, move.desiredVelocity.magnitude) * Time.deltaTime;
        move.remaining -= amountMoved;
	}

    void AdjustRotation ()
    {
        if (setFacing)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * move.turnSpeed);
    }

    void UpdateState () 
    {
        ground.stepsSinceLastGrounded += 1;
        jump.stepsSinceLastJump += 1;
		move.velocity = _rb.velocity;

		if (IsGrounded || SnapToGround() || CheckSteepContacts()) 
        {
            //TODO: MAGIC NUMBER
            if (jump.stepsSinceLastJump > 2)
            {
                _anims.StopJumping();
                jump.phase = 0;
            }

            ground.stepsSinceLastGrounded = 0;

            if (ground.groundContactCount > 1) {
				ground.contactNormal.Normalize();
			}
		}
		else 
        {
			ground.contactNormal = Vector3.up;
		}
	}

    bool SnapToGround () {
        if (jetpackActive)
            return false;
            
		if (ground.stepsSinceLastGrounded > 1 || jump.stepsSinceLastJump <= 2) {
			return false;
		}
		float speed = move.velocity.magnitude;
		if (speed > ground.maxSnapSpeed) {
			return false;
		}

        if (!Physics.Raycast(_rb.position, Vector3.down, out RaycastHit hit, ground.probeDistance)) {
			return false;
		}

        if (hit.normal.y < ground.minGroundDotProduct) {
			return false;
		}

		ground.contactNormal = hit.normal;
		float dot = Vector3.Dot(move.velocity, hit.normal);
		if (dot > 0f) {
			move.velocity = (move.velocity - hit.normal * dot).normalized * speed;
		}
		return true;
	}

    void OnCollisionStay (Collision collision) => EvaluateCollision(collision);

    void EvaluateCollision (Collision collision) 
    {
		for (int i = 0; i < collision.contactCount; i++) 
        {
			Vector3 normal = collision.GetContact(i).normal;

            if (normal.y >= ground.minGroundDotProduct) 
            {
				ground.groundContactCount += 1;
				ground.contactNormal += normal;
			}
            //TODO: MAGIC NUMBERS
            else if (normal.y > -0.01f && normal.y < 0.05f) 
            {
				ground.wallContactCount += 1;
				ground.wallNormal += normal;
			}
		}
	}

    bool CheckSteepContacts () 
    {
		if (ground.wallContactCount > 1) {
			ground.wallNormal.Normalize();
			if (ground.wallNormal.y >=ground.minGroundDotProduct)
            {
				ground.groundContactCount = 1;
				ground.contactNormal = ground.wallNormal;
				return true;
			}
		}
		return false;
	}

    public void ResupplyMove()
    {
        move.remaining = move.maxMove;
    }
}
