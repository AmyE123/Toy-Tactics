using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    public enum PlayerStatus { Idle, PlayerControlled, ComputerControlled }


    [Header("Subcomponents")]
    [SerializeField] private PlayerMove _movement;
    [SerializeField] private PlayerVisuals _visuals;
    [SerializeField] private SoldierAnimations _anims;
    [SerializeField] private PlayerEquipment _equipment;
    [SerializeField] private PlayerAI _ai;
    [SerializeField] private PlayerCameras _cameras;
    [SerializeField] private PlayerHumanInput _humanInput;

    [Space(16)]

    [SerializeField]
    private PlayerStatus _status;

    [SerializeField]
    private Transform _podium;

    [SerializeField]
    private Rigidbody _rigidBody;

    [Header("Health Properties")]   
    [SerializeField] GameObject playerUIPrefab; 
    [SerializeField] private int playerHealth;
    [SerializeField] private int _maxHealth;
    private PlayerUI playerUI;

    [Header("Team Data")]   
    [SerializeField] private TeamData _teamData;
    private int _playerId;
    private string _playerName;

    public void SetCameraFirstPerson() => _cameras.SetCameraFirstPerson();

    public void SetCameraThirdPerson() => _cameras.SetCameraThirdPerson();

    // Exposing things publicly
    public PlayerMove Movement => _movement;
    public PlayerVisuals Visuals => _visuals;
    public SoldierAnimations Animations => _anims;
    public PlayerAI AI => _ai;
    public PlayerHumanInput HumanInput => _humanInput;
    public PlayerCameras Cameras => _cameras;
    public PlayerEquipment Equipment => _equipment;
    public PlayerStatus Status => _status;
    public int MaxHealth => _maxHealth;
    public float MovePercent => _movement.MovePercent;
    public TeamData Team => _teamData;
    public PlayerUI UI => playerUI;

    public bool isTurnOver;

    float _originalMass;
    
    public int PlayerHealth
    {
        get{ return playerHealth; }
        set{ playerHealth = value; }
    }


    public void SetPlayerID(int pid)
    {
        _playerId = pid;

        if (_teamData == null)
            return;

        if (_teamData.memberNames.Length <= pid)
        {
            _playerName = $"Solider {pid + 1}";
        }
        else
        {
            _playerName = _teamData.memberNames[pid];
        }

        if (playerUI != null)
            playerUI.setPlayerName(_playerName);
    }

    public bool IsGrounded => _movement.IsGrounded;

    public void SetJetpackActive(bool visualState, bool actualState)
    {
        _movement.SetJetpackActive(actualState);
        _visuals.SetJetpackActive(visualState, actualState);
    }

    void Start()
    {
        SetPlayerStatus(PlayerStatus.Idle); 
        PlayerHealth = _maxHealth;   
        _visuals.SetPlayerMaterial(_teamData);
        _originalMass = _rigidBody.mass;
        _anims.SetAsStatic();
    }

    void Update()
    {
        if (_status == PlayerStatus.PlayerControlled)
        {
            _humanInput.DoUpdate();
            _equipment.PlayerInput();
        }

        if (playerHealth > 0 && transform.position.y < -10)
        {
            W2CManager.DoDamageBurst(playerHealth, this, transform.position);
            playerHealth = 0;
            playerUI.SetHealth(playerHealth, _maxHealth);
            PlayerDeath();
        }
    }

    public void SpawnPlayerUI()
    {
        playerUI = W2C.InstantiateAs<PlayerUI>(playerUIPrefab);
        playerUI.SetPosition(transform, Vector3.up);
        playerUI.setPlayerName(_playerName);
        playerUI.Init(this);

        if (_teamData != null)
            playerUI.SetColor(_teamData.textColor);
    } 

    void LockPodiumPosition()
    {
        _podium.transform.position = transform.position - (transform.up * 0.8f);
        _podium.transform.rotation = transform.rotation;
    }

    public void AIControlPlayer()
    {       
        SetPlayerStatus(PlayerStatus.ComputerControlled);
        _ai.DoStart();
        _anims.SetNotStatic();
        _cameras.SetCameraThirdPerson();
        isTurnOver = false;
    }

    public void TakeControlOfPlayer()
    {       
        _cameras.SetCameraThirdPerson();
        _movement.ResupplyMove();
        SetPlayerStatus(PlayerStatus.PlayerControlled);
        Cursor.lockState = CursorLockMode.Locked;
        _anims.SetNotStatic();
        isTurnOver = false;
    }

    public void StopControllingPlayer()
    {
        Equipment.UnequipItem();
        _movement.move.desiredVelocity = Vector3.zero;
        SetPlayerStatus(PlayerStatus.Idle);
        Cursor.lockState = CursorLockMode.None;
        _anims.SetAsStatic();  
        SetBootsInactive();
    }

    public IEnumerator StopControllingWithDelay(float delay, System.Action onComplete)
    {
        Equipment.UnequipItem();
        _movement.move.desiredVelocity = Vector3.zero;
        SetPlayerStatus(PlayerStatus.Idle);
        _cameras.SetCameraThirdPerson();
        Cursor.lockState = CursorLockMode.None;
        _anims.SetAsStatic();  
        SetBootsInactive();

        yield return new WaitForSeconds(delay);

        _cameras.DisableAllCameras();
        onComplete.Invoke();
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetPlayerStatus(PlayerStatus status)
    {
        _status = status;

        if (_podium == null)
            return;

        if (Status == PlayerStatus.Idle)
        {
            transform.position += new Vector3(0, 0.1f, 0);
            _podium.gameObject.SetActive(true);
            LockPodiumPosition();
            _rigidBody.constraints = RigidbodyConstraints.None;
        }
        else
        {
            _podium.gameObject.SetActive(false);
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (Status != PlayerStatus.PlayerControlled)
        {
            _cameras.DisableAllCameras();
        }
    }
    
    public void SetBootsActive(float mass, float gravityMultiplier)
    {
        _rigidBody.mass = mass;
        _movement.gravityMultiplier = gravityMultiplier;
        _visuals.SetBootsActive(true);
    }

    public void SetBootsInactive()
    {
        _rigidBody.mass = _originalMass;
        _movement.gravityMultiplier = 1;
        _visuals.SetBootsActive(false);
    }

    // If the player gets damaged or healed, this is dealt
    // with in this function. The Damage/Heal value can 
    // take in a positive or negative value.
    public void HealthValueAdjust(int damageOrHealValue, Collision col=null)
    {       
        Vector3 hitPoint = transform.position;

        if (col != null && col.contacts.Length > 0)
            hitPoint = col.contacts[0].point;

        if (damageOrHealValue < 0)
            W2CManager.DoDamageBurst((int) -damageOrHealValue, this, hitPoint);

        PlayerHealth += damageOrHealValue;
        PlayerHealth = Mathf.Clamp(PlayerHealth, 0, _maxHealth);

        playerUI.SetHealth(playerHealth, _maxHealth);

        if(PlayerHealth <= 0)
        {
            _visuals.SpawnGibs();
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        gameObject.SetActive(false);

        if (_status != PlayerStatus.Idle)
            FindObjectOfType<PlayerOrganiser>().FinishTurnIfActivePlayer(this, 2.5f);            
    }

    public void DoExplosiveDamage(Vector3 position, float radius, float force, float damageMultiplier=1)
    {
        float distance = Vector3.Distance(transform.position, position);
        float distancePercent = Mathf.Sqrt(Mathf.Clamp01(distance / radius));

        // Standing right next to it does 650, so this should be about 92 at point blank
        float damageVal = ((1 - distancePercent) * force) / 7;
        W2CManager.DoDamageBurst((int) damageVal, this, transform.position);
        
        damageVal *= damageMultiplier;

        PlayerHealth -= (int) damageVal;
        PlayerHealth = Mathf.Clamp(PlayerHealth, 0, _maxHealth);

        playerUI.SetHealth(playerHealth, _maxHealth);

        if(PlayerHealth <= 0)
        {
            _visuals.SpawnGibsWithForce(position, force, radius);
            PlayerDeath();
        }
    }

}
