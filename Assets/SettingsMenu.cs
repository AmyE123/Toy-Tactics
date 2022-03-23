using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _mouseSensSlider;
    [SerializeField] private Toggle _invertX;
    [SerializeField] private Toggle _invertY;

    [SerializeField] private GameSettings _settings;

    public void MusicSliderChanged(float val)
    {
        _settings.musicVolume = val;
        MusicManager.UpdateVolumeBasedOnSettings();
    }

    public void SoundSliderChanged(float val)
    {
        _settings.soundVolume = val;
    }

    public void MouseSliderChanged(float val)
    {
        _settings.mouseSensitivity = val;
    }

    public void InvertXChanged(bool newState)
    {
        _settings.invertX = newState;
    }

    public void InvertYChanged(bool newState)
    {
        _settings.invertY = newState;
    }

    public void SaveSettings()
    {
        _settings.SaveToPrefs();
    }

    public void SyncUI()
    {
        _musicSlider.value = _settings.musicVolume;
        _sfxSlider.value = _settings.soundVolume;
        _mouseSensSlider.value = _settings.mouseSensitivity;

        _invertX.isOn = _settings.invertX;
        _invertY.isOn = _settings.invertY;
    }

    // Start is called before the first frame update
    void Start()
    {
        _settings.LoadPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
