using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Reglas");
    }

    public void Continue()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
