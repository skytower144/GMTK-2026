using TMPro;
using UnityEngine;

public class SignalCounterDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private GameObject walkIcon, holdIcon;

    public void HandleUpdate(bool isRunning, float displayNumber)
    {
        DisplayRemainingTime(isRunning, displayNumber);
        DisplayIcon(isRunning);
    }

    private void DisplayRemainingTime(bool isRunning, float displayNumber)
    {
        numberText.gameObject.SetActive(isRunning);
        numberText.text = $"{displayNumber:F0}";
    }

    private void DisplayIcon(bool isRunning)
    {
        walkIcon.SetActive(isRunning);
        holdIcon.SetActive(!isRunning);
    }
}
