using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private GameSettings _settings;
    [SerializeField] private AudioSource _musicPlayer;
    [SerializeField] private float _volumeMultiplier;

    private static MusicManager _instance;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _musicPlayer.Play();

        UpdateVolumeBasedOnSettings();
        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateVolumeBasedOnSettings()
    {
        _instance._musicPlayer.volume = _instance._settings.MusicVolume * _instance._volumeMultiplier;
    }
}
