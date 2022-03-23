using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementBar : MonoBehaviour
{
    [SerializeField] private RectTransform _thisRect;
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private PlayerOrganiser _playerMan; 

    // Update is called once per frame
    void Update()
    {
        if (_playerMan.ActivePlayer == null)
            _barRect.localScale = new Vector3(0, 1, 1);
        else
            _barRect.localScale = new Vector3(_playerMan.ActivePlayer.MovePercent, 1, 1);
    }
}
