using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] GameSettings _settings;

    // Start is called before the first frame update
    void Start()
    {
        int i=0;

        foreach (LevelData lvl in _settings.levels)
        {
            GameObject newObj = Instantiate(_buttonPrefab, transform);
            newObj.GetComponent<PlayLevelButton>().Init(lvl, i);
            i++;
        }   
    }
}
