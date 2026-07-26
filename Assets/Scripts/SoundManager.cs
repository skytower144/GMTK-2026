using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip backgroundMusic;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (backgroundMusic != null && musicSource != null)
            PlayMusic(backgroundMusic);
    }

    public static void PlayMusic(AudioClip musicClip)
    {
        instance.musicSource.clip = musicClip;
        instance.musicSource.loop = true;
        instance.musicSource.Play();
    }

    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && instance.sfxSource != null)
        {
            instance.sfxSource.PlayOneShot(clip, volume);
        }
    }
}
