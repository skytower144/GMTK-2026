using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string mainMenuActionMapName, playerActionMapName;
    [SerializeField] private string confirmActionName;
    [SerializeField] private List<GameObject> displayList;
    private int displayIndex = 0;

    void Awake()
    {
        InputSystem.actions.FindActionMap(playerActionMapName).Disable();
        InputSystem.actions.FindActionMap(mainMenuActionMapName).Enable();
    }

    void Start()
    {
        for (int i = 0; i < displayList.Count; i++)
            displayList[i].SetActive(false);

        displayList[0].SetActive(true);
    }

    void Update()
    {
        if (GameManager.CurrentGameState == GameState.MENU && InputSystem.actions[confirmActionName].WasPressedThisFrame())
            Confirm();
    }

    private void Confirm()
    {
        if (displayIndex >= displayList.Count - 1)
            StartGame();
        else
        {
            displayList[displayIndex].SetActive(false);
            ++displayIndex;
            displayList[displayIndex].SetActive(true);
        }
    }

    private void StartGame()
    {
        InputSystem.actions.FindActionMap(playerActionMapName).Enable();
        InputSystem.actions.FindActionMap(mainMenuActionMapName).Disable();

        GameManager.instance.StartGame();
    }
}
