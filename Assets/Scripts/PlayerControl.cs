using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private const string ANIMPARAM_MOVE_X = "moveX";
    private const string ANIMPARAM_MOVE_Y = "moveY";
    private const string ANIMPARAM_ISMOVING = "isMoving";
    private const float ATTACK_LINGER_DURATION = 0.4f;

    public enum PlayerState { IDLE, MOVE, ATTACK, INTERACT, }
    public static event Action OnRetryButtonDown = null;

    [Space(5), Header("Actions")]
    [SerializeField] private string moveActionName;
    [SerializeField] private string attackActionName;
    [SerializeField] private string retryActionName;

    [Space(5), Header("Animations")]
    [SerializeField] private string idleAnimName;
    [SerializeField] private string attackAnimName;

    [Space(10)]
    [SerializeField] private float attackDuration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject attackCollider;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 currentMoveVector;
    private Coroutine attackCoroutine;

    public PlayerState CurrentState { get; private set; }
    public bool IsMoving => IsActionPressed(moveActionName);

    void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
        rb = GetComponentInChildren<Rigidbody2D>(true);
        sr = GetComponentInChildren<SpriteRenderer>(true);
    }

    void OnEnable()
    {
        CurrentState = PlayerState.IDLE;

#if UNITY_EDITOR
        GameManager.SetInputAction(true);
#endif
    }

    void Update()
    {
        AnimateFacingDirection();

        if (IsActionPressed(attackActionName))
        {
            Attack();
        }
        else if (GameManager.CurrentGameState == GameState.LEVEL_FAIL && IsActionPressed(retryActionName))
        {
            OnRetryButtonDown?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (CurrentState == PlayerState.IDLE || CurrentState == PlayerState.MOVE)
            ProcessMove();
    }

    private void ProcessMove()
    {
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
    }

    private void AnimateFacingDirection()
    {
        Vector2 snappedVector = GetSnappedVector(currentMoveVector);
        anim.SetFloat(ANIMPARAM_MOVE_X, snappedVector.x);
        anim.SetFloat(ANIMPARAM_MOVE_Y, snappedVector.y);
    }

    private void Attack()
    {
        if (attackCoroutine != null)
            return;
        
        attackCoroutine = StartCoroutine(Routine());
        IEnumerator Routine()
        {
            CurrentState = PlayerState.ATTACK;

            anim.Play(attackAnimName, -1, 0f);
            yield return new WaitForSeconds(attackDuration);

            CurrentState = PlayerState.IDLE;
            anim.Play(idleAnimName, -1, 0f);
            attackCoroutine = null;
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
        return InputSystem.actions[actionName].IsPressed();
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
    private void SpawnAttackCollider(string direction)
    {
        var spawnedCollider = Instantiate(attackCollider, transform);
        spawnedCollider.transform.localPosition = Vector3.zero;

        direction = direction.ToUpper();

        switch (direction)
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

        Destroy(spawnedCollider, ATTACK_LINGER_DURATION);
    }

}
