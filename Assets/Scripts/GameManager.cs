using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private int initialCountdown;
    public static int CurrentCountdown { get; private set; }

    void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Detected multiple GameManagers");
            Destroy(gameObject);
            return;
        }

        instance = this;

        CurrentCountdown = initialCountdown;
    }

    void Update()
    {
        
    }
}
