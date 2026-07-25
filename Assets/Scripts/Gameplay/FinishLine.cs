using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private BoxCollider2D collider;

    void Awake()
    {
        collider = GetComponentInChildren<BoxCollider2D>(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameManager.PLAYER_TAG))
        {
            GameManager.instance.DetermineLevelComplete();
            SetTrigger(false);
        }
    }

    public void SetTrigger(bool state)
    {
        collider.enabled = state;
    }
}
