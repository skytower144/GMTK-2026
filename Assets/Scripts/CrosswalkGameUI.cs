using DG.Tweening;
using UnityEngine;

public class CrosswalkGameUI : MonoBehaviour
{
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject levelCompleteText, levelFailText;
    [field: SerializeField] public DOTweenAnimation FadeScreenTween;

    private GameObject spawnedResultText = null;

    public void DisplayLevelResultText(bool isLevelComplete)
    {
        GameObject spawningText = isLevelComplete ? levelCompleteText : levelFailText;

        if (spawnedResultText != null)
            Destroy(spawnedResultText);
        
        spawnedResultText = Instantiate(spawningText, canvasTransform);
    }

    public void DestroySpawnedResultText()
    {
        if (spawnedResultText != null)
            Destroy(spawnedResultText);
    }

    public void FadeScreen()
    {
        FadeScreenTween.DORewind();
        FadeScreenTween.DOPlay();
    }
}
