using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoundUI : MonoBehaviour
{
    [SerializeField] CanvasGroup _grp;
    [SerializeField] RectTransform _rect;
    [SerializeField] Text _yourTurnText;
    [SerializeField] Text _teamNameText;
    [SerializeField] Text _roundNumText;
    
    public void ShowData(TeamData team, int roundNum, System.Action OnComplete)
    {
        _teamNameText.text = team.TeamName.ToUpper();
        _roundNumText.text = $"TURN {roundNum + 1}";
        _yourTurnText.text = "IT'S YOUR TURN";
        
        if (team.IsComputerControlled)
            _yourTurnText.text = "CPU PLAYERS TURN";

        _teamNameText.color = team.TextColor;
        _roundNumText.color = team.TextColor;

        StartCoroutine(ShowAnimation(OnComplete));
    }

    void TextFadeIn(Text text, float delay)
    {
        Color col = text.color;
        col.a = 0;
        text.color = col;

        text.DOFade(1, 0.7f).SetEase(Ease.OutExpo).SetDelay(delay);
    }

    IEnumerator ShowAnimation(System.Action OnComplete)
    {
        _grp.alpha = 0;
        _rect.localScale = new Vector3(0.9f, 0.0f, 0.9f);

        _rect.DOScale(1, 0.8f).SetEase(Ease.OutExpo);
        _grp.DOFade(1, 0.8f).SetEase(Ease.OutExpo);

        TextFadeIn(_yourTurnText, 0.5f);
        TextFadeIn(_teamNameText, 0.8f);
        TextFadeIn(_roundNumText, 1.5f);

        yield return new WaitForSeconds(3);

        _rect.DOScale(new Vector3(1f, 0.0f, 1f), 0.5f).SetEase(Ease.InSine);
        _grp.DOFade(0, 0.5f).SetEase(Ease.InSine);

        yield return new WaitForSeconds(0.5f);

        OnComplete.Invoke();
    }
}
