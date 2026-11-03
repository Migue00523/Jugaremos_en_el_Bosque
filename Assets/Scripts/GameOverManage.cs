using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    public bool IsGameOver { get; private set; }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

}