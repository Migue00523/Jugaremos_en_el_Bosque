
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void OpenOptions()
    {
        Debug.Log("Abrir menú de opciones");
    }
}
