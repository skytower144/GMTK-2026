using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private const string ANIMPARAM_MOVE_X = "moveX";
    private const string ANIMPARAM_MOVE_Y = "moveY";
    private const string ANIMPARAM_ISMOVING = "isMoving";
    private const float COLLIDER_LINGER_DURATION = 0.4f;
    private const float HURT_STUN_DURATION = 0.28f;
    private const float HURT_KNOCKBACK_SPEED = 10f;

    public enum PlayerState { IDLE, MOVE, ATTACK, INTERACT, HURT, }
    public static event Action OnRetryButtonDown = null;

    [Space(5), Header("Actions")]
    [SerializeField] private string moveActionName;
    [SerializeField] private string attackActionName;
    [SerializeField] private string retryActionName;

    [Space(5), Header("Animations")]
    [SerializeField] private string idleAnimName;
    [SerializeField] private string attackAnimName;
    [SerializeField] private string interactAnimName;

    [Space(10)]
    [SerializeField] private float attackDuration;
    [SerializeField] private float interactDuration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject attackCollider, interactCollider;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip hurtSfx;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 currentMoveVector;
    private Coroutine animateCoroutine, stunCoroutine;

    public PlayerState CurrentState { get; private set; }
    public bool IsMoving => InputSystem.actions[moveActionName].IsPressed();

    void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
        rb = GetComponentInChildren<Rigidbody2D>(true);
        sr = GetComponentInChildren<SpriteRenderer>(true);
    }

    void OnEnable()
    {
        CurrentState = PlayerState.IDLE;
    }

    void Update()
    {
        AnimateFacingDirection();

        if (GameManager.CurrentGameState == GameState.LEVEL_FAIL && IsActionPressed(retryActionName))
        {
            OnRetryButtonDown?.Invoke();
            return;
        }

        if (CurrentState == PlayerState.HURT)
            return;
        
        if (IsActionPressed(attackActionName))
        {
            Animate(PlayerState.ATTACK, attackDuration);
        }
        else if (IsActionPressed(interactAnimName))
        {
            Animate(PlayerState.INTERACT, interactDuration);
        }
    }

    void FixedUpdate()
    {
        if (CurrentState == PlayerState.IDLE || CurrentState == PlayerState.MOVE)
            ProcessMove();
    }

    private void ProcessMove()
    {
        PlayerState prevState = CurrentState;
        anim.SetBool(ANIMPARAM_ISMOVING, IsMoving);

        if (IsMoving)
        {
            currentMoveVector = InputSystem.actions[moveActionName].ReadValue<Vector2>();
            sr.flipX = currentMoveVector.x < 0;

            Vector2 nextPos = rb.position + moveSpeed * Time.fixedDeltaTime * currentMoveVector;
            rb.MovePosition(nextPos);

            CurrentState = PlayerState.MOVE;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            CurrentState = PlayerState.IDLE;
        }

        if (prevState != CurrentState)
        {
            if (IsMoving)
                footstepSource.Play();
            else
                footstepSource.Stop();
        }
    }

    private void AnimateFacingDirection()
    {
        Vector2 snappedVector = GetSnappedVector(currentMoveVector);
        anim.SetFloat(ANIMPARAM_MOVE_X, snappedVector.x);
        anim.SetFloat(ANIMPARAM_MOVE_Y, snappedVector.y);
    }

    private void Animate(PlayerState state, float duration)
    {
        if (animateCoroutine != null)
            return;
        
        animateCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            CurrentState = state;

            string animName = string.Empty;

            switch (state)
            {
                case PlayerState.ATTACK:
                    animName = attackActionName;
                    break;
                
                case PlayerState.INTERACT:
                    animName = interactAnimName;
                    break;
                
                default:
                    break;

            }
            anim.Play(animName, -1, 0f);
            yield return new WaitForSeconds(duration);

            CurrentState = PlayerState.IDLE;
            anim.Play(idleAnimName, -1, 0f);
            animateCoroutine = null;
        }
    }

    private Vector2 GetSnappedVector(Vector2 inputVector)
    {
        return new Vector2(
            Mathf.RoundToInt(inputVector.x),
            Mathf.Round(inputVector.y)
        );
    }

    private bool IsActionPressed(string actionName)
    {
        return InputSystem.actions[actionName].WasPressedThisFrame();
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void SetCurrentMoveVector(Vector3 inputVector)
    {
        currentMoveVector = inputVector;
    }

    // Animation Key Event
    private void SpawnCollider(string direction)
    {
        direction = direction.ToUpper();
        string[] info = direction.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

        GameObject targetPrefab;

        if (info.Length != 2)
        {
            Debug.LogWarning($"Invalid parameter for animation event: SpawnCollider");
            return;
        }

        switch (info[0])
        {
            case "ATTACK":
                targetPrefab = attackCollider;
                break;
            
            default:
            case "INTERACT":
                targetPrefab = interactCollider;
                break;
        }

        var spawnedCollider = Instantiate(targetPrefab, transform);
        spawnedCollider.transform.localPosition = Vector3.zero;

        switch (info[1])
        {
            case "UP":
                spawnedCollider.transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            
            case "RIGHT":
                float zValue = currentMoveVector.x > 0 ? 90f : -90f;
                spawnedCollider.transform.rotation = Quaternion.Euler(0, 0, zValue);
                break;
            
            default:
                break;
        }

        Destroy(spawnedCollider, COLLIDER_LINGER_DURATION);
    }

    public void Knockback()
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        
        stunCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            CurrentState = PlayerState.HURT;
            SoundManager.PlaySFX(hurtSfx);
            
            Vector2 approximateDestination = (Vector2)transform.position + -currentMoveVector;

            for (float t = 0f; t < HURT_STUN_DURATION; t += Time.fixedDeltaTime)
            {
                Vector2 nextPos = Vector2.MoveTowards(rb.position, approximateDestination, HURT_KNOCKBACK_SPEED * Time.fixedDeltaTime);
                rb.MovePosition(nextPos);

                yield return new WaitForFixedUpdate();
            }

            CurrentState = PlayerState.IDLE;
            stunCoroutine = null;
        }
    }

    public void Trapped(Vector3 trapPosition, float trappedDuration)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        
        Vector2 originalVector = currentMoveVector;

        stunCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            CurrentState = PlayerState.HURT;
            SoundManager.PlaySFX(hurtSfx);

            anim.SetBool(ANIMPARAM_ISMOVING, false);
            SetPosition(trapPosition);

            float magnitude = 0.1f;

            for (float t = 0f; t < trappedDuration; t += Time.deltaTime)
            {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * magnitude;
                SetPosition(new Vector3(
                    trapPosition.x + randomOffset.x,
                    trapPosition.y + randomOffset.y,
                    trapPosition.z
                ));

                yield return null;
            }

            Vector2 approximateDestination = (Vector2)transform.position + -originalVector;

            for (float t = 0f; t < HURT_STUN_DURATION; t += Time.fixedDeltaTime)
            {
                Vector2 nextPos = Vector2.MoveTowards(rb.position, approximateDestination, HURT_KNOCKBACK_SPEED * Time.fixedDeltaTime);
                rb.MovePosition(nextPos);

                yield return new WaitForFixedUpdate();
            }

            CurrentState = PlayerState.IDLE;
            stunCoroutine = null;
        }
    }
}
