using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Unknown, Movement, Gun, Explosive, Projectile }

    public WeaponType weaponType;
    public string displayName;
    public Sprite icon;
    public string description;
    public GameObject equipPrefab;

    [Header("Bot Stuff")]
    public float waitTimeForBot;
    public bool canBotUse;
    public float attackRange;
    
    [Header("End of turn stuff")]
    public bool willEndTurn;
}
