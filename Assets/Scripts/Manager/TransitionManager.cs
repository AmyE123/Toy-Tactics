using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    const float LOAD_SCENE_DELAY = 0.15f;

    private static TransitionManager _instance;

    [SerializeField] private Image _topImage;
    [SerializeField] private Image _bottomImage;
    [SerializeField] private float _transitionTime;
    [SerializeField] private GameSettings _settings;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public static void GoToMenu() => _instance.StartTransition("MainMenu");

    public static void GoToGame() => _instance.StartTransition("GameScene");

    public static void GoToMapNumber(int levelId)
    {
        _instance._settings.selectedLevel = levelId;
        LevelData level = _instance._settings.GetCurrentLevel();

        if (level == null)
            GoToMenu();
        else
            GoToGame();
    }

    public static void GoToNextMap()
    {
        _instance._settings.selectedLevel ++;
        LevelData level = _instance._settings.GetCurrentLevel();

        if (level == null)
            GoToMenu();
        else
            GoToGame();
    }

    private void StartTransition(string sceneName)
    {
        Cursor.lockState = CursorLockMode.None;
        _topImage.DOFillAmount(1, _transitionTime).SetEase(Ease.Linear).OnComplete(() => LoadScene(sceneName));
        _bottomImage.DOFillAmount(1, _transitionTime).SetEase(Ease.Linear);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        _bottomImage.DOFillAmount(0, _transitionTime).SetEase(Ease.Linear).SetDelay(LOAD_SCENE_DELAY);
        _topImage.DOFillAmount(0, _transitionTime).SetEase(Ease.Linear).SetDelay(LOAD_SCENE_DELAY);
        Cursor.lockState = CursorLockMode.None;
    }
}
