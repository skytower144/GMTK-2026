using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private const string ANIMPARAM_MOVE_X = "moveX";
    private const string ANIMPARAM_MOVE_Y = "moveY";
    private const string ANIMPARAM_ISMOVING = "isMoving";

    [SerializeField] private string moveActionName;
    [SerializeField] private float moveSpeed;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 currentMoveVector;
    public bool IsMoving => InputSystem.actions[moveActionName].IsPressed();

    void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
        rb = GetComponentInChildren<Rigidbody2D>(true);
        sr = GetComponentInChildren<SpriteRenderer>(true);
    }

    void FixedUpdate()
    {
        ProcessMove();
    }

    private void ProcessMove()
    {
        anim.SetBool(ANIMPARAM_ISMOVING, IsMoving);

        if (IsMoving)
        {
            currentMoveVector = InputSystem.actions[moveActionName].ReadValue<Vector2>();
            sr.flipX = currentMoveVector.x < 0;

            anim.SetFloat(ANIMPARAM_MOVE_X, currentMoveVector.x);
            anim.SetFloat(ANIMPARAM_MOVE_Y, currentMoveVector.y);

            Vector2 nextPos = rb.position + moveSpeed * Time.fixedDeltaTime * currentMoveVector;
            rb.MovePosition(nextPos);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
