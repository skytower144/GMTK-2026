using CrosswalkGame;
using TMPro;
using UnityEngine;

public class SignalCounterDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private GameObject walkIcon, holdIcon;
    private GameTimer crosswalkTimer => GameManager.instance.CrosswalkTimer;
    private bool canWalk
    {
        get
        {
            if (crosswalkTimer == null)
                return false;
            
            return crosswalkTimer.MaxTime - crosswalkTimer.ElapsedTime > 0f;
        }
    }

    void Update()
    {
        DisplayRemainingTime();
        DisplayIcon();
    }

    private void DisplayRemainingTime()
    {
        if (crosswalkTimer == null)
            return;
        
        numberText.gameObject.SetActive(canWalk);
        numberText.text = $"{crosswalkTimer.MaxTime - crosswalkTimer.ElapsedTime:F0}";
    }

    private void DisplayIcon()
    {
        walkIcon.SetActive(canWalk);
        holdIcon.SetActive(!canWalk);
    }
}
