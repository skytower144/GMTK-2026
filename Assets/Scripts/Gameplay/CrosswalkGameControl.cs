using System.Collections;
using System.Collections.Generic;
using CrosswalkGame;
using Unity.Cinemachine;
using UnityEngine;

public class CrosswalkGameControl : MonoBehaviour
{
    private const float LEVEL_RESULT_WAIT_DURATION = 3f;
    public static int CurrentLevel { get; private set; } = 0;
    public PlayerControl PlayerControl => player;

    [SerializeField] private PlayerControl player;
    [SerializeField] private CrosswalkGameUI uiControl;
    [SerializeField] private SignalCounterDisplay worldCrosswalkSignal, uiCrosswalkSignal;
    [SerializeField] private FinishLine finishLineControl;
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private CinemachineFollow cinemachineFollow;
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private GameObject endingCutscene;
    [SerializeField] private AudioClip backgroundMusic, endingMusic, completeSfx, failedSfx;

    [Space(10)]
    [SerializeField] private float crosswalkTimelimit;

    [Space(10)]
    [SerializeField] private float showFinishlineDuration;
    [SerializeField] private Vector2 showFinishlineDamping;
    [SerializeField] private List<GameObject> levelPrefabList;

    public GameTimer CrosswalkTimer { get; private set; }
    private Vector2 initialFollowDamping;
    private GameObject currentLevelInstance = null;

    private Coroutine levelResultCoroutine = null;
    private Coroutine proceedLevelCoroutine = null;

    void Awake()
    {
        GameManager.SetInputAction(false);
        SetCurrentLevel(0);

        CrosswalkTimer = new GameTimer(crosswalkTimelimit);
        initialFollowDamping = cinemachineFollow.TrackerSettings.PositionDamping;

        PlayerControl.OnRetryButtonDown -= RetryLevel;
        PlayerControl.OnRetryButtonDown += RetryLevel;
    }

    void OnDestroy()
    {
        PlayerControl.OnRetryButtonDown -= RetryLevel;
    }

    void Start()
    {
        ProceedLevel(1);
        SoundManager.PlayMusic(backgroundMusic);
    }

    void Update()
    {
        DetermineLevelFail();
        worldCrosswalkSignal.HandleUpdate(CrosswalkTimer.IsRunning, CrosswalkTimer.LeftTimeUntilMaxed);
        uiCrosswalkSignal.HandleUpdate(CrosswalkTimer.IsRunning, CrosswalkTimer.LeftTimeUntilMaxed);
    }

    private void RetryLevel()
    {
        ProceedLevel(CurrentLevel);
    }

    private void ShowEnding()
    {
        var ending = Instantiate(endingCutscene, uiControl.transform);
        ending.transform.localPosition = Vector3.zero;

        SoundManager.PlayMusic(endingMusic);
    }

    public void ProceedLevel(int level)
    {
        if (proceedLevelCoroutine != null)
            StopCoroutine(proceedLevelCoroutine);

        proceedLevelCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            GameManager.SetGameState(GameState.LEVEL_PREPARE);
            GameManager.SetInputAction(false);

            uiControl.FadeScreen();
            uiControl.DestroySpawnedResultText();

            CrosswalkTimer.Rewind();

            if (level > levelPrefabList.Count)
            {
                GameManager.SetGameState(GameState.FINISHED);
                ShowEnding();
                yield break;
            }

            SetCurrentLevel(level);
            LoadLevel(CurrentLevel);

            player.SetPosition(playerStartPosition.position);
            player.SetCurrentMoveVector(Vector2.up);
            finishLineControl.SetTrigger(true);

            yield return new WaitForSeconds(uiControl.FadeScreenTween.delay);
            yield return ShowFinishline();

            CrosswalkTimer.Run();

            GameManager.SetGameState(GameState.PLAY);
            GameManager.SetInputAction(true);
        }

        IEnumerator ShowFinishline()
        {
            cinemachineFollow.TrackerSettings.PositionDamping = showFinishlineDamping;
            yield return new WaitForFixedUpdate();

            cinemachineCam.Follow = finishLineControl.transform;
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(showFinishlineDuration);

            cinemachineCam.Follow = player.transform;
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(showFinishlineDuration);

            cinemachineFollow.TrackerSettings.PositionDamping = initialFollowDamping;
        }

        proceedLevelCoroutine = null;
    }


    public void DetermineLevelComplete()
    {
        if (GameManager.CurrentGameState != GameState.PLAY)
            return;
        
        if (CrosswalkTimer.IsRunning)
            CompleteLevel();
    }

    private void DetermineLevelFail()
    {
        if (GameManager.CurrentGameState != GameState.PLAY)
            return;
        
        if (!CrosswalkTimer.IsRunning)
            FailLevel();
    }

    public void CompleteLevel()
    {
        if (levelResultCoroutine != null)
            StopCoroutine(levelResultCoroutine);

        levelResultCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            GameManager.SetGameState(GameState.LEVEL_COMPLETE);
            uiControl.DisplayLevelResultText(isLevelComplete: true);
            SoundManager.PlaySFX(completeSfx);

            yield return new WaitForSeconds(LEVEL_RESULT_WAIT_DURATION);
            ProceedLevel(CurrentLevel + 1);

            levelResultCoroutine = null;
        }
    }

    public void FailLevel()
    {
        GameManager.SetGameState(GameState.LEVEL_FAIL);
        uiControl.DisplayLevelResultText(isLevelComplete: false);
        SoundManager.PlaySFX(failedSfx);
    }

    public static void SetCurrentLevel(int level)
    {
        CurrentLevel = level;
    }

    public void LoadLevel(int level)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        if (level - 1 >= levelPrefabList.Count)
        {
            Debug.LogWarning($"Failed to load Level: {level - 1}");
            return;
        }

        if (levelPrefabList[level - 1])
            currentLevelInstance = Instantiate(levelPrefabList[level - 1]);
    }
}