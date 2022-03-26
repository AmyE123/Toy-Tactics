using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchController : MonoBehaviour
{
    public enum MatchState { Start, ShowTurn, ChooseCharacter, WaitingForChoose, TakeTurn, TurnFinished, TeamWon, GameDraw }

    [SerializeField] MatchState _state;

    [SerializeField] TeamManager _teamManager;
    [SerializeField] PlayerOrganiser _playerManager;
    [SerializeField] UIManager _uiManager;

    [SerializeField] int _currentTeamNum;
    [SerializeField] int _currentRound;

    [SerializeField] float _sceneWarmupTime = 1.1f;

    bool _initialised = false;

    bool _endScreenTriggered = false;

    public MatchState State => _state;

    [SerializeField] GameSettings _settings;
    LevelData _level;
    List<TeamData> _teamList;

    public void FinishTurn() => _state = MatchState.TurnFinished;

    public void StartTurn()
    {
        _state = MatchState.TakeTurn;
        _uiManager.HideTeamHealth();
    }
    
    public void Init()
    {
        _initialised = true;
        _level = _settings.GetCurrentLevel();
        _teamList = _teamManager.AllTeams.OrderBy(x => Random.value).ToList();

        if (_level.teamOrder.Count > 0)
        {
            _teamList = _level.teamOrder;
        }
    }

    void HandleStart()
    {
        if (_sceneWarmupTime > 0)
        {
            _sceneWarmupTime -= Time.deltaTime;
            return;
        }

        _state = MatchState.ShowTurn;

        TeamData team = _teamList[_currentTeamNum];
        _uiManager.ShowTurn(team, _currentRound, OnTurnShown);
        _uiManager.ShowTeamHealth();
    }

    void HandleChooseCharacter()
    {
        TeamData team = _teamList[_currentTeamNum];

        if (team.isComputerControlled)
        {
            Player chosenPlayer = _teamManager.GetRandomMemberOfTeam(team);
            _playerManager.SelectPlayer(chosenPlayer);
        }
        else
        {
            _playerManager.ShowButtonsForTeam(team);
            _state = MatchState.WaitingForChoose;
        }
    }

    void HandleWinScreen()
    {
        TeamData winningTeam = _teamManager.CalculateTotalTeamHealth().SortedListOfTeams()[0];

        _endScreenTriggered = true;
        _uiManager.ShowWinScreen(winningTeam, _currentRound);
    }

    void HandleDrawScreen()
    {
        _endScreenTriggered = true;
        _uiManager.ShowDrawScreen(_currentRound);
    }

    void Update()
    {
        if (_initialised == false)
            return;

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            TransitionManager.GoToMenu();

        if (_state == MatchState.Start)
            HandleStart();

        if (_state == MatchState.ChooseCharacter)
            HandleChooseCharacter();

        if (_state == MatchState.TurnFinished)
            SwapToNextTeam();
  
        if (_state == MatchState.TeamWon && _endScreenTriggered == false)  
            HandleWinScreen();
  
        if (_state == MatchState.GameDraw && _endScreenTriggered == false)
            HandleDrawScreen();
    }

    void SwapToNextTeam()
    {
        var teamStats = _teamManager.CalculateTotalTeamHealth();
        int remainingTeams = 0;

        foreach (int healthLeft in teamStats.teamCurrentHealth.Values)
        {
            if (healthLeft > 0)
                remainingTeams ++;
        }

        Debug.Log($"There are {remainingTeams} teams left");

        if (remainingTeams == 1)
        {
            _state = MatchState.TeamWon;
            return;
        }

        if (remainingTeams == 0)
        {
            _state = MatchState.GameDraw;
            return;
        }

        _currentTeamNum ++;

        if (_currentTeamNum >= _teamList.Count)
        {
            _currentTeamNum = 0;
            _currentRound ++;
        }

        TeamData currentTeam = _teamList[_currentTeamNum];

        if (teamStats.teamCurrentHealth[currentTeam] <= 0)
        {
            SwapToNextTeam();
            return;
        }
        _state = MatchState.Start;
    }

    void OnTurnShown()
    {
        _state = MatchState.ChooseCharacter;
    }

}
