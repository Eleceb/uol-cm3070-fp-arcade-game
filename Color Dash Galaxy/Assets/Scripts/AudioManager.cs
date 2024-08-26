using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Tracks")]
    public AudioClip backgroundMusic;
    public AudioClip bossMusic;
    public AudioClip winMusic;
    public AudioClip gameOverMusic;

    [Header("Game Sounds")]
    public AudioClip shootingSound;
    public AudioClip changeColorSound;
    public AudioClip explosionSound;
    public AudioClip explodingSound;
    public AudioClip bigExplosionSound;
    public AudioClip bossGetHitSound;

    [Header("Button Sounds")]
    public AudioClip buttonSelectSound;
    public AudioClip buttonPressSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource explodingSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize music and SFX audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        explodingSource = gameObject.AddComponent<AudioSource>();
        explodingSource.clip = explodingSound;
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayExplodingSound()
    {
        explodingSource.Play();
    }

    public void StopExplodingSound()
    {
        explodingSource.Stop();
    }
}
