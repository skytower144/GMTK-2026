using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private UnityEvent onFinishlineTouch;
    private BoxCollider2D collider;

    void Awake()
    {
        collider = GetComponentInChildren<BoxCollider2D>(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameManager.PLAYER_TAG))
        {
            onFinishlineTouch?.Invoke();
        }
    }

    public void SetTrigger(bool state)
    {
        collider.enabled = state;
    }
}
