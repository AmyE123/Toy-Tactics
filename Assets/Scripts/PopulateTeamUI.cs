using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PopulateTeamUI : MonoBehaviour
{
    [SerializeField] private TeamManager _teamManager;
    [SerializeField] private TeamHealthBar[] _healthBars;

    [SerializeField] private float _activeX;
    [SerializeField] private float _inactiveX;
    [SerializeField] private float _transitionTime;
    
    RectTransform _rect;

    public void Appear()
    {
        Refresh();
        _rect.DOAnchorPosX(_activeX, _transitionTime).SetEase(Ease.OutExpo);
    }

    public void Disappear()
    {
        _rect.DOAnchorPosX(_inactiveX, _transitionTime).SetEase(Ease.OutExpo);
    }

    IEnumerator Start()
    {
        _rect = GetComponent<RectTransform>();
        yield return new WaitForSeconds(0.1f);
        Appear();
    }

    public void Refresh()
    {
        TeamHealthInfo info = _teamManager.CalculateTotalTeamHealth();
        int numTeams = info.teamMaxHealth.Count;

        for (int i=0; i<_healthBars.Length; i++)
            _healthBars[i].gameObject.SetActive(i < numTeams);

        
        int j = 0;
        
        foreach (TeamData team in info.SortedListOfTeams())
        {
            int currentHealth = info.teamCurrentHealth[team];
            int maxHealth = info.teamMaxHealth[team];
            _healthBars[j].SetInfo(team, currentHealth, info.maxHealthOfAnyTeam);

            j ++;
        }
    }
}
