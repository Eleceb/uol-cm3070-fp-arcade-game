using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenus : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text timeText;
    [SerializeField] Text inGameTimeText;

    LevelManager levelManager;

    private void OnEnable()
    {
        levelManager = FindObjectOfType<LevelManager>();
        scoreText.text = "Score: " + levelManager.score.ToString();
        timeText.text = inGameTimeText.text;
    }

    public void PlayAgainButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PressButton()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.buttonPressSound);
        AudioManager.Instance.StopMusic();
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
