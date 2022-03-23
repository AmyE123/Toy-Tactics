using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPanel : MonoBehaviour
{
    [SerializeField] InputField _teamName;
    [SerializeField] InputField[] _characterNames;
    [SerializeField] Toggle _cpuControl;
    [SerializeField] Graphic[] _toTheme;
    [SerializeField] Graphic _background;
    [SerializeField] TeamData _team;

    // Start is called before the first frame update
    public void ShowDataOnUi()
    {
        _team.LoadPrefs();
        _teamName.text = _team.teamName;
        
        for (int i=0; i<4; i++)
        {
            if (i < _team.memberNames.Length)
            {
                _characterNames[i].text = _team.memberNames[i];
            }   
            else
            {
                _characterNames[i].text = "";
            }         
        }

        foreach (Graphic g in _toTheme)
        {
            g.color = _team.textColor;
        }

        _background.color = _team.color;
        _cpuControl.isOn = _team.isComputerControlled;
    }

    public void Sync()
    {
        _team.teamName = _teamName.text;

        _team.memberNames = new string[4];
        _team.memberNames[0] = _characterNames[0].text;
        _team.memberNames[1] = _characterNames[1].text;
        _team.memberNames[2] = _characterNames[2].text;
        _team.memberNames[3] = _characterNames[3].text;

        _team.isComputerControlled = _cpuControl.isOn;

        _team.SaveToPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
