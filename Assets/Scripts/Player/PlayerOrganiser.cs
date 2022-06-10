using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrganiser : MonoBehaviour
{
    const float TURN_SKIP_COOLDOWN_COUNT = 5f;

    private List<Player> _players;

    private bool _turnSkipCooldown = false;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private Player _activePlayer;

    public Player ActivePlayer => _activePlayer;

    // Start is called before the first frame update
    public void Init()
    {
        _players = new List<Player>(FindObjectsOfType<Player>());
    }

    // Update is called once per frame
    void Update()
    {
        if (_activePlayer != null && _activePlayer.Team.IsComputerControlled == false && Input.GetKeyDown(KeyCode.T) && _turnSkipCooldown == false)
        {
            FinishTurnForActivePlayer();
            Invoke("TurnSkipCooldownReset", TURN_SKIP_COOLDOWN_COUNT);
            _turnSkipCooldown = true;
        }           
    }

    void TurnSkipCooldownReset()
    {
        _turnSkipCooldown = false;
    }

    public void SelectPlayer(Player player)
    {
        if (_activePlayer != null)
            _activePlayer.StopControllingPlayer();
        
        _activePlayer = player;

        if (player.Team.IsComputerControlled)
            player.AIControlPlayer();
        else
            player.TakeControlOfPlayer();

        foreach (Player p in _players)
            p.UI.HideButton();

        FindObjectOfType<MatchController>().StartTurn();
    }

    public void FinishTurnIfActivePlayer(Player player, float delay=1)
    {
        if (_activePlayer != player)
            return;

        StartCoroutine(_activePlayer.StopControllingWithDelay(delay, OnFinishTurnComplete));
    }

    void OnFinishTurnComplete()
    {
        _activePlayer = null;
        FindObjectOfType<MatchController>().FinishTurn();
    }

    public void FinishTurnForActivePlayer()
    {
        if (_activePlayer != null)
        {
            _activePlayer.StopControllingPlayer();
            StartCoroutine(_activePlayer.StopControllingWithDelay(1f, OnFinishTurnComplete));
        }
        else
        {
            OnFinishTurnComplete();
        }
    }

    public void FinishTurnForPlayerIn(Player player, float time)
    {
        StartCoroutine(FinishTurnWait(player, time));
    }

    IEnumerator FinishTurnWait(Player player, float time)
    {
        yield return new WaitForSeconds(time);

        FinishTurnIfActivePlayer(player);
    }

    public void ShowButtonsForTeam(TeamData team)
    {
        foreach (Player p in _players)
        {
            if (p.PlayerHealth < 0 || p.Team != team)
                continue;

            p.UI.ShowButton();
        }
    }
}
