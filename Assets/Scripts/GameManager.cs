using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;

public enum GameState { MENU, PLAY, LEVEL_COMPLETE, LEVEL_FAIL, PAUSED }
public class GameManager : MonoBehaviour
{
    public const string PLAYER_TAG = "Player";

    public static GameManager instance { get; private set; }
    public static GameState CurrentGameState = GameState.MENU;
    public static int CurrentLevel { get; private set; } = 1;
    
    [field:SerializeField] public UIController UIControl { get; private set; }
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string GameplaySceneName;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SetGameState(GameState.MENU);
    }

    public static void SetInputAction(bool state)
    {
        if (state)
            instance.inputAction.Enable();
        else
            instance.inputAction.Disable();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(GameplaySceneName);
    }

    public static void SetGameState(GameState state)
    {
        CurrentGameState = state;
    }

    public static void SetCurrentLevel(int level)
    {
        CurrentLevel = level;
    }
}
