using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenus : MonoBehaviour
{
    [SerializeField] Text finalScoreText;

    LevelsManager levelManager;

    private void OnEnable()
    {
        levelManager = FindObjectOfType<LevelsManager>();
        finalScoreText.text = "Final Score: " + levelManager.score.ToString();
    }

    public void PlayAgainButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButtonPressed()
    {
        SceneManager.LoadScene("Menu");
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
