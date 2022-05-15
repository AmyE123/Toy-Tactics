using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour
{
    private const float END_TURN_FAILSAFE_DELAY = 8f;
    private const int CLOSEST_DIST = 9999;

    private const float LOW_RAY_Y = 0.6f;
    private const float HIGH_RAY_Y = 2f;
    private const float LOW_HIT_DIST = 1.5f;
    private const float HIGH_HIT_DIST = 2;

    [SerializeField] private Player _player;
    [SerializeField] private PlayerMove _move;
    [SerializeField] private WeaponData _chosenWeapon;

    [Header("Values")]
    [SerializeField] private int _aiTargetRangeMin = 5;
    [SerializeField] private int _aiTargetRangeMax = 13;
    [SerializeField] private float _aiMinRunTime = 10f;

    [Header("Timings")]
    [SerializeField] float _startWaitTime = 2;
    [SerializeField] float _lockOnTime = 0.5f;

    float currentRunTime;
    float currentLockOnTime;
    float randomRangeGen;

    Vector3 _targetPosition;
    Player _targetPlayer;

    void EndMyTurn()
    {
        FindObjectOfType<PlayerOrganiser>().FinishTurnForPlayerIn(_player, 1);
    }

    public void DoStart()
    {
        currentRunTime = 0;
        currentLockOnTime = 0;
        _targetPlayer = null;

        StartCoroutine(PickTargetRoutine());
    }

    IEnumerator PickTargetRoutine()
    {
        yield return new WaitForSeconds(_startWaitTime);

        //find closest weakest player
        //access playerorganizer/playermanager to get team manager use find of type
        _targetPlayer = FindClosestWeakTeamPlayer();

        // Something has gone wrong, finish turn
        if (_targetPlayer == null)
        {
            EndMyTurn();
            yield break;
        }

        _targetPosition = _targetPlayer.transform.position;

        randomRangeGen = Random.Range(_aiTargetRangeMin, _aiTargetRangeMax);
        StartCoroutine(GoToTargetRoutine());
    }

    IEnumerator GoToTargetRoutine()
    {
        while (true)
        {
            bool reachedTarget = RunToTargetPosition();
            CheckIfCanJump();

            if (reachedTarget)
            {
                //stop the bot
                _move.move.desiredVelocity = Vector3.zero;
                StartCoroutine(ChooseWeaponRoutine());
                break;
            }

            // Wait until the next frame
            yield return null;
        }
    }

    IEnumerator ChooseWeaponRoutine()
    {
        //TODO: Write this better
        int randomNum = Random.Range(0, 3);

        if (randomNum == 2)
            _chosenWeapon = _player.Equipment.inventory.TryGetExplosive();
        
        if (randomNum == 1)
            _chosenWeapon = _player.Equipment.inventory.TryGetProjectile();

        if (randomNum == 0 || _chosenWeapon == null)
            _chosenWeapon = _player.Equipment.inventory.TryGetGun();

        // If we didn't find a weapon, give up
        if (_chosenWeapon == null)
        {
            EndMyTurn();
            yield break;
        }

        _player.Equipment.EquipItem(_chosenWeapon); 

        StartCoroutine(AimWeaponRoutine());
    }

    IEnumerator AimWeaponRoutine()
    {
        while (true)
        {
            bool lockedOn = _player.Cameras.AimFirstPersonCamera(_targetPlayer.transform.position);

            if (lockedOn)
                currentLockOnTime += Time.deltaTime;

            if (currentLockOnTime > _lockOnTime)
            {
                StartCoroutine(ShootWeaponRoutine());
                yield break;
            }

            // wait a frame
            yield return null;
        }
    }

    IEnumerator ShootWeaponRoutine()
    {
        _player.Equipment.UseEquippedWeapon();
        
        StartCoroutine(EndTurnFailsafe());
        yield break;
    }

    IEnumerator EndTurnFailsafe()
    {
        yield return new WaitForSeconds(END_TURN_FAILSAFE_DELAY);

        if (_player.Status != Player.PlayerStatus.Idle)
        {
            EndMyTurn();
        }
    }

    Player FindClosestPlayer()
    {
        Player closest = null;
        float closestDist = 99999;

        foreach (Player other in FindObjectsOfType<Player>())
        {
            // Don't want to target self
            if (other.Team == _player.Team)
                continue;

            float distTo = Vector3.Distance(other.transform.position, transform.position);

            if (distTo < closestDist)
            {
                closest = other;
                closestDist = distTo;
            }
        }

        return closest;
    }

    Player FindClosestWeakTeamPlayer()
    {
        Player closest = null;
        float closestDist = CLOSEST_DIST;

        var teamHealthInfo = FindObjectOfType<TeamManager>().CalculateTotalTeamHealth();
        var teamList = teamHealthInfo.SortedListOfTeams();
        var strongest = teamList[0];

        if(_player.Team == strongest)
        {
            strongest = teamList[1];
        }

        foreach (Player other in FindObjectsOfType<Player>())
        {
            if(other.Team == strongest)
            {
                // Don't want to target self
                if (other.Team == _player.Team)
                    continue;

                float distTo = Vector3.Distance(other.transform.position, transform.position);

                if (distTo < closestDist)
                {
                    closest = other;
                    closestDist = distTo;
                }
            }

        }

        return closest;

    }

    bool RunToTargetPosition()
    {
        Vector3 dirToTarget = _targetPosition - transform.position;
        dirToTarget.y = 0;
        dirToTarget.Normalize();              

        _move.move.desiredVelocity = dirToTarget * _move.move.maxSpeed;
        _move.move.desiredVelocity.y = 0;
        _player.Animations.SetAsRunning();

        _move.targetRot = Quaternion.LookRotation(_move.move.desiredVelocity, Vector3.up);
        _move.setFacing = true;

        float rangeToPlayer = Vector3.Distance(_targetPosition, transform.position);
        
        currentRunTime += Time.deltaTime;

        if (rangeToPlayer <= randomRangeGen || currentRunTime > _aiMinRunTime)
        {
            return true;
        }
        
        return false;
    }

    void CheckIfCanJump()
    {
        RaycastHit lowHit;
        RaycastHit highHit;

        Ray lowRay = new Ray(transform.position - new Vector3(0f, LOW_RAY_Y, 0f) , transform.forward);
        Ray highRay = new Ray(transform.position + new Vector3(0f, HIGH_RAY_Y, 0f) , transform.forward);

        bool didHitLow = Physics.Raycast(lowRay, out lowHit, LOW_HIT_DIST);
        bool didHitHigh = Physics.Raycast(highRay, out highHit, HIGH_HIT_DIST);

        float lowHitDistance = lowHit.distance;

        if(didHitLow && lowHit.collider.tag != "Player")
        {
            if(!didHitHigh)
            {  
                _move.jump.isRequested = true;
            }
        }

        DebugDrawRay();
    }

    void DebugDrawRay()
    {
        Debug.DrawRay(transform.position - new Vector3(0f, LOW_RAY_Y, 0f), transform.forward, Color.black);
        Debug.DrawRay(transform.position + new Vector3(0f, HIGH_RAY_Y, 0f), transform.forward, Color.red);
    }
}
