
using UnityEngine;

public class BushHideTrigger : MonoBehaviour
{
    private PlayerHidingController player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHidingController>();

        if (player != null)
            player.SetBushNearby(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHidingController exitingPlayer =
            other.GetComponent<PlayerHidingController>();

        if (exitingPlayer != null)
            exitingPlayer.SetBushNearby(false);
    }
}
