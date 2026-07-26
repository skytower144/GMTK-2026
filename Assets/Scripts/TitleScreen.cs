using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string mainMenuActionMapName, playerActionMapName;
    [SerializeField] private string enterGameActionName;

    void Awake()
    {
        InputSystem.actions.FindActionMap(playerActionMapName).Disable();
        InputSystem.actions.FindActionMap(mainMenuActionMapName).Enable();
    }

    void Update()
    {
        if (GameManager.CurrentGameState == GameState.MENU && InputSystem.actions[enterGameActionName].WasPressedThisFrame())
        {
            InputSystem.actions.FindActionMap(playerActionMapName).Enable();
            InputSystem.actions.FindActionMap(mainMenuActionMapName).Disable();

            GameManager.instance.StartGame();
        }
    }
}
