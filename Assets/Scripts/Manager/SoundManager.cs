using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] GameSettings _settings;
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundManager>();
            return _instance;
        }
    }

    public void PlaySound(AudioClip sound, Vector3 pos, float volume)
    {
        PlaySoundWithPitch(sound, pos, volume, 1);
    }

    public void PlaySoundWithRandomPitch(AudioClip sound, Vector3 pos, float volume, float minPitch, float maxPitch)
    {
        PlaySoundWithPitch(sound, pos, volume, Random.Range(minPitch, maxPitch));
    }

    public void PlaySoundWithPitch(AudioClip sound, Vector3 pos, float volume, float pitch)
    {
        GameObject spawnedObj = new GameObject($"Sound Effect ({sound.name})");
        AudioSource audio = spawnedObj.AddComponent<AudioSource>();
        audio.volume = volume * _settings.SoundVolume;
        audio.pitch = pitch;
        audio.PlayOneShot(sound);

        Destroy(spawnedObj, (sound.length / pitch) + 1);
    }
}
