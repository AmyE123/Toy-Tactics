using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamHealthBar : MonoBehaviour
{
    [SerializeField] private Image _progBar;
    [SerializeField] private Text _teamName;

    public void SetInfo(TeamData team, int currentHealth, int maxHealth)
    {
        _progBar.fillAmount = (float) currentHealth / (float) maxHealth;
        _teamName.text = team.TeamName;
        _progBar.color = new Color(team.Color.r, team.Color.g, team.Color.b, _progBar.color.a);
    }
}
