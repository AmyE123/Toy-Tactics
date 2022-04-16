using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelSlot : MonoBehaviour
{
    public WeaponData weaponData;

    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _slotImage;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private int _slotNum;

    [Header("Color")]
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activeColor;

    [Header("Size")]
    [SerializeField] private float _inactiveSize;
    [SerializeField] private float _activeSize;

    public EquipmentAmount amount;
    public bool isCancel;
    public bool isSelected;

    public void SetEquipData(TeamInventory inv)
    {
        bool hasSlot = false;

        foreach (var itm in inv.items)
        {
            if (itm.slotNum == _slotNum)
            {
                amount = itm;

                if (amount.amount == 0)
                    break;
                    
                weaponData = itm.data;
                _slotImage.sprite = itm.data.icon;
                hasSlot = true;
                break;
            }
        }

        if (hasSlot == false)
        {
            amount = null;
            weaponData = null;
        }

        _slotImage.gameObject.SetActive(weaponData != null);
    }

    void Update()
    {
        float lerpSpeed = Time.deltaTime * 12;
        bool isValid = weaponData != null || isCancel;

        if (isSelected && isValid)
        {
            _bgImage.color = Color.Lerp(_bgImage.color, _activeColor, lerpSpeed);
            _rect.localScale = Vector3.Lerp(_rect.localScale, new Vector3(_activeSize, _activeSize, _activeSize), lerpSpeed);
        }
        else
        {
            _bgImage.color = Color.Lerp(_bgImage.color, _inactiveColor, lerpSpeed);
            _rect.localScale = Vector3.Lerp(_rect.localScale, new Vector3(_inactiveSize, _inactiveSize, _inactiveSize), lerpSpeed);
        }
    }
}
