using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private PlayerMove _move;
    [SerializeField] private WeaponData _chosenWeapon;


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
        randomRangeGen = Random.Range(5f, 13f);
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
        yield return new WaitForSeconds(8);

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
        float closestDist = 99999;

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

        if(rangeToPlayer <= randomRangeGen || currentRunTime > 10)
        {
            return true;
        }
        
        return false;
    }

    void CheckIfCanJump()
    {
        RaycastHit lowHit;
        RaycastHit highHit;

        Ray lowRay = new Ray(transform.position - new Vector3(0f, 0.6f, 0f) , transform.forward);
        Ray highRay = new Ray(transform.position + new Vector3(0f, 2f, 0f) , transform.forward);

        bool didHitLow = Physics.Raycast(lowRay, out lowHit, 1.5f);
        bool didHitHigh = Physics.Raycast(highRay, out highHit, 2f);

        float lowHitDistance = lowHit.distance;

        if(didHitLow && lowHit.collider.tag != "Player")
        {
            if(!didHitHigh)
            {  
                _move.jump.isRequested = true;
            }
        }

        Debug.DrawRay(transform.position - new Vector3(0f, 0.6f, 0f) , transform.forward, Color.black);
        Debug.DrawRay(transform.position + new Vector3(0f, 2f, 0f) , transform.forward, Color.red);

    }
}
