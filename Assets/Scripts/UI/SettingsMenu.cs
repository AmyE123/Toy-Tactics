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
        _settings.MusicVolume = val;
        MusicManager.UpdateVolumeBasedOnSettings();
    }

    public void SoundSliderChanged(float val)
    {
        _settings.SoundVolume = val;
    }

    public void MouseSliderChanged(float val)
    {
        _settings.MouseSensitivity = val;
    }

    public void InvertXChanged(bool newState)
    {
        _settings.InvertX = newState;
    }

    public void InvertYChanged(bool newState)
    {
        _settings.InvertY = newState;
    }

    public void SaveSettings()
    {
        _settings.SaveToPrefs();
    }

    public void SyncUI()
    {
        _musicSlider.value = _settings.MusicVolume;
        _sfxSlider.value = _settings.SoundVolume;
        _mouseSensSlider.value = _settings.MouseSensitivity;

        _invertX.isOn = _settings.InvertX;
        _invertY.isOn = _settings.InvertY;
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
