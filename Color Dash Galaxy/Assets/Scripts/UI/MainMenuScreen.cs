using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, settingsMenu;
    [SerializeField] Button mainMenuDefaultButton, settingsMenuDefaultButton;
    [SerializeField] Text difficultyText;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundEffectSlider;

    int difficultyInt; // 0: Easy, 1: Normal, 2: Hard

    private void Start()
    {
        difficultyInt = PlayerPrefs.GetInt("Difficulty", 0);
        MatchDifficultyTextWithDifficultyInt();

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        soundEffectSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1);

        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);

        GoToMainMenu();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("IsSurvivalMode", -1);
        SceneManager.LoadScene("PlayScene");
    }

    public void StartSurvivalMode()
    {
        PlayerPrefs.SetInt("IsSurvivalMode", 1);
        SceneManager.LoadScene("PlayScene");
    }

    public void GoToSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenuDefaultButton.GetComponent<ButtonSelect>().isFirstDefaultButtonSelection = true;
        settingsMenuDefaultButton.Select();
    }

    public void GoToMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        mainMenuDefaultButton.GetComponent<ButtonSelect>().isFirstDefaultButtonSelection = true;
        mainMenuDefaultButton.Select();
    }

    public void DifficultyButton()
    {
        difficultyInt += 1;
        if (difficultyInt > 2)
            difficultyInt = 0;

        MatchDifficultyTextWithDifficultyInt();

        PlayerPrefs.SetInt("Difficulty", difficultyInt);
    }
    private void MatchDifficultyTextWithDifficultyInt()
    {
        switch (difficultyInt)
        {
            case 0:
                difficultyText.text = "Easy";
                break;
            case 1:
                difficultyText.text = "Normal";
                break;
            case 2:
                difficultyText.text = "Hard";
                break;
        }
    }

    private void OnMusicSliderValueChanged(float value)
    {
        AudioManager.Instance.musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", AudioManager.Instance.musicSource.volume);
    }

    private void OnSoundEffectSliderValueChanged(float value)
    {
        AudioManager.Instance.sfxSource.volume = value;
        AudioManager.Instance.explodingSource.volume = value;

        PlayerPrefs.SetFloat("SfxVolume", AudioManager.Instance.sfxSource.volume);
    }

    public void HowToPlayButtonPressed()
    {
        AudioManager.Instance.goToAnotherScreenInMenu = true;
        SceneManager.LoadScene("Tutorial");
    }

    public void PressButton()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.buttonPressSound);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
