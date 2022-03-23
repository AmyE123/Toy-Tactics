using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentAmount
{
    public int slotNum;
    public WeaponData data;
    public int amount;

    public EquipmentAmount()
    {

    }

    public EquipmentAmount(EquipmentAmount prev)
    {
        slotNum = prev.slotNum;
        data = prev.data;
        amount = prev.amount;
    }
}

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Transform _equipParent;
    [SerializeField] private GameSettings _settings;
    
    public TeamInventory inventory;
    public EquipableItem _currentEquipitem;

    public bool IsMovementBlocked => _currentEquipitem == null ? false : _currentEquipitem.ShouldStopMovement;

    public void PlayerInput()
    {
        if (_currentEquipitem == null)
            return;

        if (Input.GetMouseButtonDown(0))
            _currentEquipitem.StartUse();
    }

    public void EquipItem(WeaponData data)
    {
        UnequipItem();
        
        if (data.equipPrefab == null)
            return;

        GameObject spawnedWeapon = Instantiate(data.equipPrefab, _equipParent);
        _currentEquipitem = spawnedWeapon.GetComponent<EquipableItem>();
        
        _currentEquipitem?.Init(_player);
        _currentEquipitem?.OnEquip();
    }

    public void UnequipItem()
    {
        if (_currentEquipitem == null)
            return;

        _currentEquipitem.OnUnequip();
        Destroy(_currentEquipitem.gameObject);
        _currentEquipitem = null;
    }

    public void UseEquippedWeapon()
    {
        _currentEquipitem.StartUse();
    }
}
