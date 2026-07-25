using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using CrosswalkGame;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public const string PLAYER_TAG = "Player";
    public const string UICONTROl_TAG = "UI_CONTROLLER";

    public enum GameState { MENU, PLAY, LEVEL_COMPLETE, LEVEL_FAIL, PAUSED }

    public static GameManager instance { get; private set; }
    public static GameState CurrentGameState = GameState.MENU;
    
    [field:SerializeField] public UIController UIControl { get; private set; }
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string GameplaySceneName;
    [SerializeField] private float crosswalkTimelimit;

    public int CurrentLevel { get; private set; } = 1;
    public GameTimer CrosswalkTimer { get; private set; }

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        inputAction.Enable();
    }

    void Update()
    {
        DetermineLevelFail();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(GameplaySceneName);

        CurrentLevel = 1;
        CurrentGameState = GameState.PLAY;

        CrosswalkTimer = new GameTimer(crosswalkTimelimit);
        CrosswalkTimer.Run();
    }

    public void DetermineLevelComplete()
    {
        if (CurrentGameState != GameState.PLAY)
            return;
        
        if (CrosswalkTimer.IsRunning)
            CompleteLevel();
    }

    private void DetermineLevelFail()
    {
        if (CurrentGameState != GameState.PLAY)
            return;
        
        if (!CrosswalkTimer.IsRunning)
            FailLevel();
    }

    public void CompleteLevel()
    {
        CurrentGameState = GameState.LEVEL_COMPLETE;
        UIControl.DisplayLevelResultText(isLevelComplete: true);
    }

    public void FailLevel()
    {
        CurrentGameState = GameState.LEVEL_FAIL;
        UIControl.DisplayLevelResultText(isLevelComplete: false);
    }
}
