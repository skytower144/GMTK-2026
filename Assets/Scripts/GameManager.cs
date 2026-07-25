using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState { MENU, PLAY, PAUSED }
    public static GameManager instance { get; private set; }
    public static GameState CurrentGameState = GameState.MENU;
    
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string GameplaySceneName;

    void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Detected multiple GameManagers");
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

    public void StartGame()
    {
        SceneManager.LoadScene(GameplaySceneName);
        CurrentGameState = GameState.PLAY;
    }
}
