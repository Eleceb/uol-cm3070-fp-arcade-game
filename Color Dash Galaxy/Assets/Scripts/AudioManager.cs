using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public bool goToAnotherScreenInMenu = false;

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

    [Header("Music Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource explodingSource;

    void Awake()
    {
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif

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

        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        sfxSource.volume = PlayerPrefs.GetFloat("SfxVolume", 1);
        explodingSource.volume = PlayerPrefs.GetFloat("SfxVolume", 1);

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
        if ((scene.name == "Menu" || scene.name == "Tutorial") && !goToAnotherScreenInMenu)
        {
            PlayMusic(menuMusic, true);
        }
        else if (scene.name == "PlayScene")
        {
            PlayMusic(gameMusic, true);
        }

        goToAnotherScreenInMenu = false;
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
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
