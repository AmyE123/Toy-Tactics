using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : W2C
{
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

        //TODO: MAGIC NUMBER
        if (currentHP == 0 && _canvasGroup.alpha > 0)
            _canvasGroup.DOFade(0, 0.8f).SetEase(Ease.Linear).SetDelay(3);
    }

    public void setPlayerName(string playerNameString)
    {
        _playerName.text = playerNameString;
    }    

    public void HideButton()
    {
        //TODO: MAGIC NUMBER
        _goButtonRect.DOScale(0, 0.5f).SetEase(Ease.InElastic).OnComplete(() => _goButtonRect.gameObject.SetActive(false));
        _goButtonRect.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        _goButtonRect.localScale = Vector3.zero;
        _goButtonRect.gameObject.SetActive(true);

        //TODO: MAGIC NUMBER
        _goButtonRect.DOScale(1, 0.5f).SetEase(Ease.OutElastic);

    }
}
