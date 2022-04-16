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
        _teamName.text = team.teamName;
        _progBar.color = new Color(team.color.r, team.color.g, team.color.b, _progBar.color.a);
    }
}
