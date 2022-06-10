using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public string LevelName;

    [TextArea]
    public string Description;

    public GameObject Prefab;

    public List<TeamData> TeamOrder;

    public bool IsTutorial;

    public List<EquipmentAmount> StartLoadout;

    public Material LevelSkybox;

    public Vector3 LevelSize;

    public Texture Wallpaper;
}
