using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private RectTransform _quitButton;

    [SerializeField] private SlidingMenu _rootMenu;
    [SerializeField] private SlidingMenu _levelSelectMenu;
    [SerializeField] private SlidingMenu _settingsMenu;
    [SerializeField] private SlidingMenu _teamMenu;

    [SerializeField] private SettingsMenu _settings;
    [SerializeField] private TeamPanel[] _teamPanels;

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            _quitButton.gameObject.SetActive(false);    
    }

    public void GoToRootMenu()
    {
        _rootMenu?.SetMenuActive(true);
        _levelSelectMenu?.SetMenuActive(false);
        _settingsMenu?.SetMenuActive(false);
        _teamMenu?.SetMenuActive(false);
    }

    public void GoToLevelSelect()
    {
        _rootMenu?.SetMenuActive(false);
        _levelSelectMenu?.SetMenuActive(true);
    }

    public void GoToTeamSettings()
    {
        _rootMenu?.SetMenuActive(false);
        _teamMenu?.SetMenuActive(true);

        foreach (var tp in _teamPanels)
            tp.ShowDataOnUi();
    }

    public void GoToSettings()
    {
        _rootMenu?.SetMenuActive(false);
        _settingsMenu?.SetMenuActive(true);
        _settings?.SyncUI();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
