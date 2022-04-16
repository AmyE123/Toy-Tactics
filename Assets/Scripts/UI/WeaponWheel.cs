using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheel : MonoBehaviour
{
    [SerializeField] private Text _amountText;
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descText;
    [SerializeField] private List<WeaponWheelSlot> _slots;
    [SerializeField] private float _deadZone;
    [SerializeField] private ScaleUI[] _childUI;
    [SerializeField] private PlayerOrganiser _playerManager;

    bool _isWheelActive = false;
    float _activeTime = 0;
    bool _unequip = false;

    WeaponData _selectedWeapon;

    void OnEnable()
    {
        SetDefaultText();
    }

    void SetDefaultText()
    {
        _amountText.text = "";
        _titleText.text = "";
        _descText.text = "SELECT AN ITEM TO EQUIP IT";
    }

    // Update is called once per frame
    void Update()
    {
        if (_isWheelActive)
            _activeTime += Time.deltaTime;
        else
            _activeTime = 0;

        float x = ((Input.mousePosition.x / Screen.width) - 0.5f) * 2;
        float y = ((Input.mousePosition.y / Screen.height) - 0.5f) * 2;
        x *= (float) Screen.width / (float) Screen.height;
        Vector2 vec = new Vector2(x, y);

        float angle = Vector2.Angle(Vector2.up, vec.normalized);

        if (x < 0)
            angle = 360 - angle;

        angle += 22.5f;
        angle = angle % 360;

        int highlightedSlot = ((int)angle / 45);

        if (vec.magnitude < _deadZone)
            highlightedSlot = -1;

        for (int i=0; i<_slots.Count; i++)
            _slots[i].isSelected = i == highlightedSlot;

        if (highlightedSlot == -1)
        {
            SetSlot(null);
        }
        else
        {
            SetSlot(_slots[highlightedSlot]);
        }
    }

    public void SetSlot(WeaponWheelSlot slot)
    {
        _unequip = false;

        if (slot == null)
        {
            _selectedWeapon = null;
            SetDefaultText();
        }
        else if (slot.isCancel)
        {
            _amountText.text = "";
            _titleText.text = "CANCEL";
            _descText.text = "DON'T EQUIP ANYTHING";
            _unequip = true;
        }
        else if (slot.weaponData != null)
        {
            if (slot.amount.amount > 999)
                _amountText.text = $"UNLIMITED";
            else
                _amountText.text = $"{slot.amount.amount} LEFT";

            _selectedWeapon = slot.weaponData;
            _titleText.text = slot.weaponData.displayName.ToUpper();
            _descText.text = slot.weaponData.description.ToUpper();
        }
        else
        {
            SetDefaultText();
        }
    }

    public void SetActiveState(bool active)
    {
        if (active &&  _playerManager.ActivePlayer.isTurnOver)
            active = false;

        if (_isWheelActive == active)
            return;

        if (active)
        {
            var teamInv = _playerManager.ActivePlayer.Equipment.inventory;

            foreach (WeaponWheelSlot slot in _slots)
            {
                if (slot.isCancel == false)
                    slot.SetEquipData(teamInv);
            }
        }

        _isWheelActive = active;

        foreach (ScaleUI ui in _childUI)
            ui.isUIActive = active;

        if (active == false)
        {
            if (_activeTime > 0.1f)
            {
                if (_unequip == true)
                {
                    _playerManager.ActivePlayer?.Equipment.UnequipItem();
                }
                else if (_selectedWeapon != null)
                {
                    _playerManager.ActivePlayer?.Equipment.EquipItem(_selectedWeapon);
                }
                
            }
        }
    }
}
