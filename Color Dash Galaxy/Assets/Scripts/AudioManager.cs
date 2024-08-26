using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;
    public AudioClip winMusic;
    public AudioClip gameOverMusic;

    [Header("Game Sounds")]
    public AudioClip shootingSound;
    public AudioClip changeColorSound;
    public AudioClip explosionSound;
    public AudioClip spacejunkExplosionSound;
    public AudioClip explodingSound;
    public AudioClip bigExplosionSound;

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

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartPlayMusic(SceneManager.GetActiveScene());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartPlayMusic(scene);
    }

    private void StartPlayMusic(Scene scene)
    {
        if (scene.name == "Menu")
        {
            PlayMusic(menuMusic);
        }
        else if (scene.name == "PlayScene")
        {
            PlayMusic(gameMusic);
        }
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
