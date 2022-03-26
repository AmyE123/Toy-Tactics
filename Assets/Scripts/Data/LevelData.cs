using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public string levelName;
    
    [TextArea]
    public string description;

    public GameObject prefab;

    public List<TeamData> teamOrder;

    public bool isTutorial;

    public List<EquipmentAmount> startLoadout;

    public Material levelSkybox;

    public Vector3 levelSize;

    public Texture wallpaper;
}
