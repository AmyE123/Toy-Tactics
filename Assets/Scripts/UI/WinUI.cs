using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WinUI : MonoBehaviour
{
    [SerializeField] CanvasGroup _grp;
    [SerializeField] RectTransform _rect;

    [SerializeField] Text _gameOverText;
    [SerializeField] Text _mapNameText;
    [SerializeField] Text _teamNameText;
    [SerializeField] CanvasGroup _buttonGroup;
    [SerializeField] GameSettings _settings;
    [SerializeField] Button[] _gameOverButtons;

    public void Start()
    {
        SetButtonsInteractable(false);
    }

    public void ShowDraw(int roundNum)
    {       
        _gameOverText.text = "EVERYONE LOSES";
        _teamNameText.text = "DRAW";
        string mapName = _settings.GetCurrentLevel().LevelName.ToUpper();

        roundNum ++;

        if (roundNum != 1)
            _mapNameText.text = $"{mapName} | {roundNum} TURNS TAKEN";
        else
            _mapNameText.text = $"{mapName} | 1 TURN TAKEN";

        _teamNameText.color = new Color(1, 1, 1, 1);
        _mapNameText.color = new Color(1, 1, 1, 0.5f);

        ShowAnimation();
    }

    public void ShowData(TeamData team, int roundNum)
    {
        _gameOverText.text = "GAME OVER";
        _teamNameText.text = team.teamName.ToUpper() + " WINS";
        string mapName = _settings.GetCurrentLevel().LevelName.ToUpper();

        roundNum ++;

        if (roundNum != 1)
            _mapNameText.text = $"{mapName} | {roundNum} TURNS TAKEN";
        else
            _mapNameText.text = $"{mapName} | 1 TURN TAKEN";

        _teamNameText.color = team.textColor;
        _mapNameText.color = team.textColor;

        ShowAnimation();
    }

    void GraphicAlphaZero(Graphic grap)
    {
        Color col = grap.color;
        col.a = 0;
        grap.color = col;
    }

    void ShowAnimation()
    {
        SetButtonsInteractable(true);

        _grp.alpha = 0;
        _rect.localScale = new Vector3(0.9f, 0.0f, 0.9f);

        _rect.DOScale(1, 0.8f).SetEase(Ease.OutExpo);
        _grp.DOFade(1, 0.8f).SetEase(Ease.OutExpo);

        _buttonGroup.alpha = 0;
        _buttonGroup.DOFade(1, 0.7f).SetEase(Ease.OutExpo).SetDelay(1f);
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (Button btn in _gameOverButtons)
        {
            btn.interactable = state;
        }
    }

    public void BtnPressMenu()
    {
        TransitionManager.GoToMenu();
        SetButtonsInteractable(false);
    }

    public void BtnPressAgain()
    {
        TransitionManager.GoToGame();
        SetButtonsInteractable(false);
    }

    public void BtnPressNext()
    {
        TransitionManager.GoToNextMap();
        SetButtonsInteractable(false);
    }
}
