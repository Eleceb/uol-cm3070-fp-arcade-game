using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, settingsMenu;
    [SerializeField] Button mainMenuDefaultButton, settingsMenuDefaultButton;
    [SerializeField] Text difficultyText;

    [SerializeField] Slider volumeSlider;

    int difficultyInt; // 0: Easy, 1: Normal, 2: Hard

    private void Start()
    {
        difficultyInt = PlayerPrefs.GetInt("Difficulty", 0);
        MatchDifficultyTextWithDifficultyInt();
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1);
        volumeSlider.value = AudioListener.volume;

        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        GoToMainMenu();
    }

    public void StartGame()
    {
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

    private void OnSliderValueChanged(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
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
