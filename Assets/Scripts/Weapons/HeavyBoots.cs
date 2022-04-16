using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBoots : EquipableItem
{
    [SerializeField] float _mass;
    [SerializeField] float _gravityMultiplier;
    [SerializeField] AudioClip _equipSound;

    bool _hasBeenUsed;
   
    public override void StartUse()
    {
        base.StartUse();

        if (_hasBeenUsed)
            return;

        _hasBeenUsed = true;
        _player.SetBootsActive(_mass, _gravityMultiplier);
        SoundManager.Instance.PlaySoundWithRandomPitch(_equipSound, _player.transform.position, 1.8f, 0.9f, 1.1f);
    }

    public void FinishUse()
    {
        _player.SetBootsInactive();
    }

    public override void OnEquip()
    {
        base.OnEquip();
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        _player.SetBootsInactive();

    }
}
