using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestLevel : MonoBehaviour
{
    public static Action OnLevelRequirementCompleted { get; private set; } = null;

    [Space(20)]
    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Transform spawnTransformParent;

    [SerializeField] private OldMan oldManControl;
    [SerializeField] private GameObject ringUIPrefab;
    private GameObject spawnedRingUIInstance = null;

    void Awake()
    {
        SpawnChests();

        OnLevelRequirementCompleted -= SpawnRingUI;
        OnLevelRequirementCompleted += SpawnRingUI;

        OnLevelRequirementCompleted -= MarkOldManDeathFlag;
        OnLevelRequirementCompleted += MarkOldManDeathFlag;
    }

    void OnDestroy()
    {
        OnLevelRequirementCompleted -= SpawnRingUI;
        OnLevelRequirementCompleted -= MarkOldManDeathFlag;

        if (spawnedRingUIInstance)
            Destroy(spawnedRingUIInstance);
    }

    private void SpawnChests()
    {
        int totalPositions = 0;

        foreach (Transform spawnTransform in spawnTransformParent)
            ++totalPositions;

        int randomIndex = UnityEngine.Random.Range(0, totalPositions);

        int i = 0;
        foreach (Transform spawnTransform in spawnTransformParent)
        {
            Chest chest = Instantiate(chestPrefab, spawnTransform.position, Quaternion.identity);
            chest.transform.SetParent(transform);
            chest.Init(hasRing: i == randomIndex);
            i++;
        }
    }

    private void SpawnRingUI()
    {
        GameObject uiCanvas = GameObject.FindGameObjectWithTag(GameManager.UI_CONTROLLER);
        if (!uiCanvas)
        {
            Debug.LogWarning($"Could not find ui canvas");
            return;
        }

        spawnedRingUIInstance = Instantiate(ringUIPrefab, uiCanvas.transform);
    }

    private void MarkOldManDeathFlag()
    {
        oldManControl.SetDeathFlag(true);
    }
}
