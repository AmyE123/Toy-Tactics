using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Teams/New Team")]
public class TeamData : ScriptableObject
{
    public string teamName;
    public bool isComputerControlled;
    public Color color;
    public Color textColor;
    public string[] memberNames;

    public void SaveToPrefs()
    {
        PlayerPrefs.SetString("TeamSettings_" + name, JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        if (PlayerPrefs.HasKey("TeamSettings_" + name))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("TeamSettings_" + name), this);
    }
}
