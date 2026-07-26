using UnityEngine;

public class WallCollisionSound : MonoBehaviour
{
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float cooldownDuration = 0.3f;

    private float elapsedTime = 0f;
    private bool isCooldown => elapsedTime < cooldownDuration; 

    void Awake()
    {
        elapsedTime = cooldownDuration;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCooldown)
            return;
        
        if (collision.gameObject.CompareTag(GameManager.PLAYER_TAG))
        {
            SoundManager.PlaySFX(sfx);
            elapsedTime = cooldownDuration;
        }
    }
}
