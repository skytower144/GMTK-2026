using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Control => GameManager.instance.UIControl;

    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject levelCompleteText, levelFailText;

    public void DisplayLevelResultText(bool isLevelComplete)
    {
        GameObject spawningText = isLevelComplete ? levelCompleteText : levelFailText;
        Destroy(Instantiate(spawningText, canvasTransform), 4f);
    }
}
