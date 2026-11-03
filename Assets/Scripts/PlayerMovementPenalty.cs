
using UnityEngine;

public class PlayerMovementPenalty : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameCycleManager gameCycleManager;
    [SerializeField] private GameOverManager gameOverManager;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (gameCycleManager == null ||
            playerMovement == null ||
            gameOverManager == null)
            return;

        if (gameOverManager.IsGameOver)
            return;

        if (gameCycleManager.IsWolfResponding &&
            playerMovement.IsTryingToMove)
        {
            gameOverManager.TriggerGameOver();
        }
    }
}
