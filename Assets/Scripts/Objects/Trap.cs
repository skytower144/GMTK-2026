using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [SerializeField] private float cooldownDuration;

    private float elapsedTime = 0f;
    protected bool isOnCooldown => elapsedTime < cooldownDuration;
    protected CrosswalkGameControl gameControl;

    protected abstract void ActivateTrap();

    void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag(GameManager.GAME_CONTROL).GetComponent<CrosswalkGameControl>();
    }

    void OnEnable()
    {
        elapsedTime = cooldownDuration;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isOnCooldown)
            return;
        
        if (collision.CompareTag(GameManager.PLAYER_TAG))
        {
            elapsedTime = 0f;
            ActivateTrap();
        }
    }
}
