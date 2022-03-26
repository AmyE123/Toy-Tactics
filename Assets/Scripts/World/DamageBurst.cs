using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DamageBurst : W2C
{
    [SerializeField] Vector2 _driftAmount;
    [SerializeField] RectTransform _rect;
    [SerializeField] Outline _outline;
    
    [Header("Timings")]
    [SerializeField] float _scaleTime;
    [SerializeField] float _aliveTime;
    [SerializeField] float _fadeTime;

    [Space(8)]
    [SerializeField] Text _text;
    
    public void Init(int damage, Color col)
    {
        _text.text = damage.ToString();
        _text.color = Color.Lerp(col, Color.white, 0.5f);;
        _outline.effectColor = col;

        Vector3 punchScale = new Vector3(1.5f, 1.5f, 1.5f);
        _rect.DOPunchScale(punchScale, _scaleTime);

        _rect.DOAnchorPos(_rect.anchoredPosition + _driftAmount, _aliveTime)
                .SetEase(Ease.Linear);

        _canvasGroup.DOFade(0, _fadeTime)
                    .SetDelay(_aliveTime - _fadeTime)
                    .SetEase(Ease.Linear);

        Destroy(gameObject, _aliveTime + 0.5f);           
    }
}