using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JetpackUI : MonoBehaviour
{
    RectTransform _rect;
    CanvasGroup _grp;

    [SerializeField] private Image _fillImage;
    [SerializeField] private Vector3 _inactiveScale;
    [SerializeField] private float _transitionTime;
    [SerializeField] private Gradient _fuelColor;

    private Jetpack _jetpack;
    private bool _isVisible;

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _grp = GetComponent<CanvasGroup>();

        _grp.alpha = 0;
        _rect.localScale = _inactiveScale;     
    }

    public void SetVisible(Jetpack jetpack)
    {
        _jetpack = jetpack;
        _isVisible = true;

        _rect.DOScale(1, _transitionTime).SetEase(Ease.OutExpo);
        _grp.DOFade(1, _transitionTime).SetEase(Ease.OutExpo);
    }

    
    public void Hide()
    {
        if (_isVisible == false)
            return;
        
        _rect.DOScale(_inactiveScale, _transitionTime).SetEase(Ease.OutExpo);
        _grp.DOFade(0, _transitionTime).SetEase(Ease.OutExpo);
        _isVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isVisible == false)
            return;

        if (_jetpack == null)
        {
            Hide();
            return;
        }

        _fillImage.fillAmount = _jetpack.FuelPercent;
        _fillImage.color = _fuelColor.Evaluate(_jetpack.FuelPercent);
    }
}
