using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlidingMenu : MonoBehaviour
{
    [SerializeField] Vector2 _activePos;
    [SerializeField] Vector2 _inactivePos;
    [SerializeField] float _transitionTime;

    [SerializeField] bool _isActive;

    public void SetMenuActive(bool active)
    {
        if (active == _isActive)
            return;

        _isActive = active;

        GetComponent<RectTransform>()
            .DOAnchorPos(active ? _activePos : _inactivePos, _transitionTime)
            .SetEase(Ease.OutExpo);
    }
}
