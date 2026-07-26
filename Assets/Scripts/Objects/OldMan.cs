using UnityEngine;

public class OldMan : MonoBehaviour
{
    [SerializeField] private Animator oldManAnim;
    [SerializeField] private Collider2D oldManCollider;
    [SerializeField] private string oldManDeathAnimName;
    [SerializeField] private GameObject displayTextObj;
    public bool DeathFlag { get; private set; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!DeathFlag)
            return;
        
        if (collision.CompareTag(GameManager.PLAYER_INTERACT_COLLIDER) || collision.CompareTag(GameManager.PLAYER_ATTACK_COLLIDER))
        {
            DisableOldMan();
        }
    }

    public void SetDeathFlag(bool state)
    {
        DeathFlag = state;
    }

    private void DisableOldMan()
    {
        oldManAnim.Play(oldManDeathAnimName, -1, 0f);
        oldManCollider.enabled = false;
        displayTextObj.gameObject.SetActive(false);
    }
}
