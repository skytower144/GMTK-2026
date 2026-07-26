using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugControl : MonoBehaviour
{
    [SerializeField] private CrosswalkGameControl gameControl;
    [SerializeField] private GameObject inputFieldObject;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string debugActionName;
    [SerializeField] private string debugmodeActionMapName;
    [SerializeField] private string playerActionMapName;

#if UNITY_EDITOR

    void Start()
    {
        inputFieldObject.SetActive(false);
        inputField.onSubmit.AddListener(OnSubmitText);
    }

    void OnDestroy()
    {
        inputField.onSubmit.RemoveListener(OnSubmitText);
    }

    void Update()
    {
        if (IsActionPressed(debugActionName))
            ToggleInputField();
    }

    private void OnSubmitText(string submittedText)
    {
        if (string.IsNullOrWhiteSpace(submittedText))
        {
            ToggleInputField();
            return;
        }

        ProcessCommand(submittedText);
        DisableInputField();
    }

    private void ProcessCommand(string submittedText)
    {
        string[] words = submittedText.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (words.Length <= 0)
            return;

        switch (words[0])
        {
            case "level":
                if (words.Length != 2)
                    return;
                
                gameControl.ProceedLevel(int.Parse(words[1]));           
                break;
            
            case "complete":
                gameControl.CompleteLevel();
                break;
            
            default:
                break;
        }
    }

    private void ToggleInputField()
    {
        bool isActive = !inputFieldObject.activeSelf;
        inputFieldObject.SetActive(isActive);

        if (isActive)
        {
            GameManager.SetInputAction(true);
            EnableInputField();
        }
        else
        {
            DisableInputField();
        }
    }

    private void EnableInputField()
    {
        inputField.ActivateInputField();
        inputField.text = string.Empty;

        InputSystem.actions.FindActionMap(playerActionMapName).Disable();
        InputSystem.actions.FindActionMap(debugmodeActionMapName).Enable();
    }

    private void DisableInputField()
    {
        inputField.text = string.Empty;
        inputFieldObject.SetActive(false);
        inputField.DeactivateInputField();

        InputSystem.actions.FindActionMap(playerActionMapName).Enable();
        InputSystem.actions.FindActionMap(debugmodeActionMapName).Disable();
    }

    private bool IsActionPressed(string actionName)
    {
        return InputSystem.actions[actionName].WasPressedThisFrame();
    }

#endif
}
