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
        _teamName.text = _team.TeamName;
        
        for (int i=0; i<4; i++)
        {
            if (i < _team.MemberNames.Length)
            {
                _characterNames[i].text = _team.MemberNames[i];
            }   
            else
            {
                _characterNames[i].text = "";
            }         
        }

        foreach (Graphic g in _toTheme)
        {
            g.color = _team.TextColor;
        }

        _background.color = _team.Color;
        _cpuControl.isOn = _team.IsComputerControlled;
    }

    public void Sync()
    {
        _team.TeamName = _teamName.text;

        _team.MemberNames = new string[4];
        _team.MemberNames[0] = _characterNames[0].text;
        _team.MemberNames[1] = _characterNames[1].text;
        _team.MemberNames[2] = _characterNames[2].text;
        _team.MemberNames[3] = _characterNames[3].text;

        _team.IsComputerControlled = _cpuControl.isOn;

        _team.SaveToPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
