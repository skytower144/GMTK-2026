using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject itemIcon;
    [SerializeField] private SpriteRenderer itemIconRenderer;
    [SerializeField] private Sprite ringSprite, coalSprite;
    public bool HasRing { get; private set; }
    public bool IsOpened { get; private set; }

    public void Init(bool hasRing)
    {
        HasRing = hasRing;
        itemIconRenderer.sprite = HasRing ? ringSprite : coalSprite;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOpened)
            return;
        
        if (collision.CompareTag(GameManager.PLAYER_INTERACT_COLLIDER))
        {
            if (HasRing)
                ChestLevel.OnLevelRequirementCompleted?.Invoke();
            
            OpenChest();
        }
    }

    private void OpenChest()
    {
        IsOpened = true;
        itemIcon.SetActive(true);
    }
}
