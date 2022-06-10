using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Teams/New Team")]
public class TeamData : ScriptableObject
{
    public string TeamName;
    public bool IsComputerControlled;
    public Color Color;
    public Color TextColor;
    public string[] MemberNames;

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
