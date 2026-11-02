
using UnityEngine;

public class PlayerHidingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterVisibility characterVisibility;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameCycleManager gameCycleManager;
    [SerializeField] private CameraFollow cameraFollow;
    
    [Header("Hidden Visual")]
    [SerializeField] private GameObject hiddenPanel;

    [Header("UI Buttons")]
    [SerializeField] private GameObject hideButton;
    [SerializeField] private GameObject exitHideButton;

    private bool isHidden = false;
    private bool isNearBush = false;

    private void Start()
    {
        if (characterVisibility == null)
            characterVisibility = GetComponent<CharacterVisibility>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (hiddenPanel != null)
            hiddenPanel.SetActive(false);

        hideButton.SetActive(false);
        exitHideButton.SetActive(false);
    }

    private void Update()
    {
        bool canHide = gameCycleManager != null &&
                       gameCycleManager.IsHidePhase;

        hideButton.SetActive(
            isNearBush && canHide && !isHidden
        );
    }

    public void SetBushNearby(bool nearby)
    {
        isNearBush = nearby;
    }

    public void Hide()
    {
        if (isHidden || !isNearBush)
            return;

        if (gameCycleManager == null ||
            !gameCycleManager.IsHidePhase)
            return;

        if (hiddenPanel != null)
            hiddenPanel.SetActive(true);

        if (characterVisibility == null)
        {
            Debug.LogError("CharacterVisibility is not assigned!");
            return;
        }

        isHidden = true;

        characterVisibility.SetHidden(true);
        if (cameraFollow != null)
            cameraFollow.SetLookMode(true);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(false);

        hideButton.SetActive(false);
        exitHideButton.SetActive(true);

        Debug.Log("Player is hiding!");
    }

    public void ExitHide()
    {
        if (!isHidden)
            return;

        isHidden = false;

        characterVisibility.SetHidden(false);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        if (hiddenPanel != null)
            hiddenPanel.SetActive(false);

        exitHideButton.SetActive(false);

        if (cameraFollow != null)
            cameraFollow.SetLookMode(false);
    }
}
