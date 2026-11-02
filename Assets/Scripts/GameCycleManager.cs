
using UnityEngine;
using System.Collections;

public class GameCycleManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip songClip;

    [Header("Wolf Responses")]
    [SerializeField] private AudioClip[] preparationResponses;
    [SerializeField] private AudioClip readyResponse;

    [Range(0f, 1f)]
    [SerializeField] private float readyChance = 0.20f;

    [Header("NPC References")]
    [SerializeField] private NPCWander[] npcs;

    [Header("Round Settings")]
    [SerializeField] private float pauseBetweenRounds = 1.5f;

    public bool IsHidePhase { get; private set; }

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (!IsHidePhase)
        {
            // SONG PHASE
            Debug.Log("The song begins!");

            SetNPCMovement(true);

            yield return PlayAudio(songClip);

            // WOLF RESPONSE PHASE
            SetNPCMovement(false);

            Debug.Log("The wolf is responding...");

            bool wolfIsReady =
                readyResponse != null &&
                (preparationResponses == null ||
                 preparationResponses.Length == 0 ||
                 Random.value < readyChance);

            if (wolfIsReady)
            {
                Debug.Log("THE WOLF IS READY!");

                yield return PlayAudio(readyResponse);

                IsHidePhase = true;

                Debug.Log("HIDE PHASE STARTED!");

                yield break;
            }

            if (preparationResponses != null &&
                preparationResponses.Length > 0)
            {
                int randomIndex = Random.Range(
                    0,
                    preparationResponses.Length
                );

                yield return PlayAudio(
                    preparationResponses[randomIndex]
                );
            }
            else
            {
                Debug.LogWarning("No preparation responses assigned.");
            }

            yield return new WaitForSeconds(pauseBetweenRounds);
        }
    }

    private IEnumerator PlayAudio(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("An audio clip is missing!");
            yield break;
        }

        audioSource.clip = clip;
        audioSource.Play();

        yield return new WaitWhile(
            () => audioSource.isPlaying
        );
    }

    private void SetNPCMovement(bool enabled)
    {
        foreach (NPCWander npc in npcs)
        {
            if (npc != null)
            {
                npc.SetMovementEnabled(enabled);
            }
        }
    }
}

