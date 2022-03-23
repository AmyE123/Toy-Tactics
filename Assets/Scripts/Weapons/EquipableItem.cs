using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipableItem : MonoBehaviour
{
    [SerializeField] WeaponData _data;

    public bool isFirstPerson;

    protected Rigidbody _playerRigidBody;
    protected Player _player;

    public virtual bool ShouldStopMovement => false;

    public virtual void Init(Player player)
    {
        _player = player;
        _playerRigidBody = player.GetComponent<Rigidbody>();
    }

    public virtual void StartUse()
    {
        FindObjectOfType<EquipControls>().Hide();
        _player.Equipment.inventory.UseItem(_data);

        if (_data.willEndTurn)
            _player.isTurnOver = true;
    }

    public virtual void OnEquip()
    {
        TeamInventory inv = _player.Equipment.inventory;
        
        if (_player.Status == Player.PlayerStatus.PlayerControlled)
        {
            FindObjectOfType<EquipControls>().SetData(_data, inv.GetAmount(_data));
        }

        if (isFirstPerson)
            _player.SetCameraFirstPerson();
    }

    public virtual void OnUnequip()
    {
        _player.SetCameraThirdPerson();
    }

    public virtual void EndTurnInSeconds(float seconds)
    {
        FindObjectOfType<PlayerOrganiser>().FinishTurnForPlayerIn(_player, seconds);
    }
}
