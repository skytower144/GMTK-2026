using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string mainMenuActionMapName, playerActionMapName;
    [SerializeField] private string confirmActionName;
    [SerializeField] private List<GameObject> displayList;
    [SerializeField] private AudioClip confirmSfx;
    private int displayIndex = 0;

    // temporary, prevent double input bug
    private float elapsedTime = 0f;
    private float inputCooldown = 0.1f;
    private bool isCooldown => elapsedTime < inputCooldown; 

    void Awake()
    {
        InputSystem.actions.FindActionMap(playerActionMapName).Disable();
        InputSystem.actions.FindActionMap(mainMenuActionMapName).Enable();
        elapsedTime = inputCooldown;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (isCooldown)
            return;
        
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

        SoundManager.PlaySFX(confirmSfx);
        elapsedTime = 0f;
    }

    private void StartGame()
    {
        InputSystem.actions.FindActionMap(playerActionMapName).Enable();
        InputSystem.actions.FindActionMap(mainMenuActionMapName).Disable();

        GameManager.instance.StartGame();
    }
}
