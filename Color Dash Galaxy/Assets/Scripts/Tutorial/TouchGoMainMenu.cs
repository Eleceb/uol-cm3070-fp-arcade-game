using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchGoMainMenu : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            AudioManager.Instance.goToAnotherScreenInMenu = true;
            SceneManager.LoadScene("Menu");
    }
}
