using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField] GameSettings _settings;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(_settings.GetCurrentLevel().prefab, transform);
        FindObjectOfType<PlayerOrganiser>().Init();
        FindObjectOfType<TeamManager>().Init();
        FindObjectOfType<MatchController>().Init();
    }
}
