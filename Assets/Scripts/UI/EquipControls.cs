using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EquipControls : MonoBehaviour
{
    [SerializeField] private Text _itemName;
    [SerializeField] private Text _explaination;
    [SerializeField] private Text _willEndTurn;
    [SerializeField] private Vector3 _inactiveScale;

    RectTransform _rect;
    CanvasGroup _grp;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _grp = GetComponent<CanvasGroup>();

        _grp.alpha = 0;
        _rect.localScale = _inactiveScale;
    }

    public void SetData(WeaponData data, int uses)
    {   
        _rect.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
        _grp.DOFade(1, 0.5f).SetEase(Ease.OutExpo);

        if (uses == 1)
        {
            _itemName.text = $"{data.displayName} (1 use left)";
        }
        else if (uses > 999)
        {
            _itemName.text = $"{data.displayName} (unlimited)";
        }
        else
        {
            _itemName.text = $"{data.displayName} ({uses} uses left)";
        }

        if (data.willEndTurn)
        {
            _willEndTurn.text = "(this will end your turn)";
        }
        else
        {
            _willEndTurn.text = "";
        }
    }

    public void Hide()
    {
        _rect.DOScale(_inactiveScale, 0.2f).SetEase(Ease.Linear);
        _grp.DOFade(0, 0.2f).SetEase(Ease.Linear);
    }
}
