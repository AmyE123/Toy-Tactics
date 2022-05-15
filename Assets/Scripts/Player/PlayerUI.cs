using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : W2C
{
    private const float DEATH_SCREEN_FADE_DURATION = 0.8f;
    private const int DEATH_TRANSITION_DELAY = 3;
    private const float BUTTON_SCALE_DURATION = 0.5f;

    [SerializeField] private Text _playerName;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _selectText;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private RectTransform _goButtonRect;

    private Player _player;
    private PlayerOrganiser _playerManager;

    public void Init(Player player)
    {
        _player = player;
        _playerManager = FindObjectOfType<PlayerOrganiser>();

        _goButtonRect.localScale = Vector3.zero;
        _goButtonRect.gameObject.SetActive(false);
    }

    public void BtnPress()
    {
        _playerManager.SelectPlayer(_player);
    }


    public void SetColor(Color col)
    {
        _playerName.color = col;
        _healthText.color = col;
        _selectText.color = col;
    }

    public void SetHealth(int currentHP, int maxHP)
    {
        float hpPercent = (float) currentHP / (float) maxHP;
        _healthBarImage.fillAmount = hpPercent;
        _healthText.text = currentHP.ToString();

        if (currentHP == 0 && _canvasGroup.alpha > 0)
            _canvasGroup.DOFade(0, DEATH_SCREEN_FADE_DURATION).SetEase(Ease.Linear).SetDelay(DEATH_TRANSITION_DELAY);
    }

    public void SetPlayerName(string playerNameString)
    {
        _playerName.text = playerNameString;
    }    

    public void HideButton()
    {
        _goButtonRect.DOScale(0, BUTTON_SCALE_DURATION).SetEase(Ease.InElastic).OnComplete(() => _goButtonRect.gameObject.SetActive(false));
        _goButtonRect.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        _goButtonRect.localScale = Vector3.zero;
        _goButtonRect.gameObject.SetActive(true);

        _goButtonRect.DOScale(1, BUTTON_SCALE_DURATION).SetEase(Ease.OutElastic);

    }
}
