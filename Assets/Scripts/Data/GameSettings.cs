using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public bool InvertX;
    public bool InvertY;

    public float MouseSensitivity = 5;

    public float MusicVolume;
    public float SoundVolume;

    public LevelData[] Levels;
    public int SelectedLevel;

    public LevelData GetCurrentLevel()
    {
        if (SelectedLevel >= Levels.Length)
            return null;
            
        return Levels[SelectedLevel];
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetString("GameSettings", JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        List<LevelData> tmpLevelDatas = new List<LevelData>(Levels);

        if (PlayerPrefs.HasKey("GameSettings"))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("GameSettings"), this);

        Levels = tmpLevelDatas.ToArray();
    }
}
