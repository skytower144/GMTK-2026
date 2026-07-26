using System.Collections;
using CrosswalkGame;
using Unity.Cinemachine;
using UnityEngine;

public class CrosswalkGameControl : MonoBehaviour
{
    [SerializeField] private PlayerControl player;
    [SerializeField] private CrosswalkGameUI uiControl;
    [SerializeField] private SignalCounterDisplay worldCrosswalkSignal, uiCrosswalkSignal;
    [SerializeField] private FinishLine finishLineControl;
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private CinemachineFollow cinemachineFollow;
    [SerializeField] private Vector2 showFinishlineDamping;
    [SerializeField] private float showFinishlineDuration;
    [SerializeField] private float crosswalkTimelimit;

    public GameTimer CrosswalkTimer { get; private set; }
    private Coroutine proceedLevelCoroutine = null;
    private Vector2 initialFollowDamping;

    void Awake()
    {
        GameManager.SetCurrentLevel(1);

        CrosswalkTimer = new GameTimer(crosswalkTimelimit);
        initialFollowDamping = cinemachineFollow.TrackerSettings.PositionDamping;
    }

    void Start()
    {
        ProceedNextLevel();
    }

    void Update()
    {
        DetermineLevelFail();
        worldCrosswalkSignal.HandleUpdate(CrosswalkTimer.IsRunning, CrosswalkTimer.LeftTimeUntilMaxed);
        uiCrosswalkSignal.HandleUpdate(CrosswalkTimer.IsRunning, CrosswalkTimer.LeftTimeUntilMaxed);
    }

    public void ProceedNextLevel()
    {
        if (proceedLevelCoroutine != null)
            StopCoroutine(proceedLevelCoroutine);

        proceedLevelCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            GameManager.SetInputAction(false);
            yield return ShowFinishline();

            CrosswalkTimer.Rewind();
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
        GameManager.SetGameState(GameState.LEVEL_COMPLETE);
        uiControl.DisplayLevelResultText(isLevelComplete: true);
    }

    public void FailLevel()
    {
        GameManager.SetGameState(GameState.LEVEL_FAIL);
        uiControl.DisplayLevelResultText(isLevelComplete: false);
    }
}
