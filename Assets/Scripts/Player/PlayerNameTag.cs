using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameTag : W2C
{
    [SerializeField]TextMeshProUGUI playerName;

    public void setPlayerName(string playerNameString)
    {
        playerName.text = playerNameString;
    }
}
