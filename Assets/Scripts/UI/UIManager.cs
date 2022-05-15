using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerOrganiser _playerMan;
    [SerializeField] private WeaponWheel _weaponWheel;
    [SerializeField] private PopulateTeamUI _teamHP;
    [SerializeField] private RoundUI _roundUI;
    [SerializeField] private WinUI _winUI;
    [SerializeField] private MatchController _matchController;
    [SerializeField] private RectTransform _crosshair;

    public void ShowTeamHealth() => _teamHP.Appear();
    
    public void HideTeamHealth() => _teamHP.Disappear();

    public void ShowTurn(TeamData team, int round, System.Action onComplete)
                               => _roundUI.ShowData(team, round, onComplete);

    public void ShowWinScreen(TeamData winningTeam, int rounds) => _winUI.ShowData(winningTeam, rounds);

    public void ShowDrawScreen(int rounds) => _winUI.ShowDraw(rounds);

    void Start()
    {
        _crosshair.localScale = Vector3.zero;
    }

    public void ShowCrosshair()
    {
        _crosshair.DOScale(0.8f, 1f).SetEase(Ease.OutExpo).SetDelay(0.8f);
    }

    public void HideCrosshair()
    {
        _crosshair.DOScale(0, 0.5f).SetEase(Ease.InExpo);
    }

    void Update() 
    {
        ManageWeaponWheel();
    }

    void ManageWeaponWheel()
    {
        bool isActive = Input.GetKey(KeyCode.E) || Input.GetMouseButton(1);
        isActive &= _playerMan.ActivePlayer != null;

        if (isActive)
            Cursor.lockState = CursorLockMode.None;
        
        // TODO: rather than doing this every frame better to do only on dismiss
        if (_playerMan.ActivePlayer != null && isActive == false)
            Cursor.lockState = CursorLockMode.Locked;

        _weaponWheel.SetActiveState(isActive);
    }
}
