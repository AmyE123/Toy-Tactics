using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public bool invertX;
    public bool invertY;

    public float mouseSensitivity = 5;

    public float musicVolume;
    public float soundVolume;

    public LevelData[] levels;
    public int selectedLevel;

    public LevelData GetCurrentLevel()
    {
        if (selectedLevel >= levels.Length)
            return null;
            
        return levels[selectedLevel];
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetString("GameSettings", JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        List<LevelData> tmpLevelDatas = new List<LevelData>(levels);

        if (PlayerPrefs.HasKey("GameSettings"))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("GameSettings"), this);

        levels = tmpLevelDatas.ToArray();
    }
}
