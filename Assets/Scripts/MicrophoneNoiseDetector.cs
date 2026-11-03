using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicrophoneNoiseDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject noisePanel;
    [SerializeField] private Slider noiseSlider;
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Microphone Settings")]
    [SerializeField] private int sampleWindow = 128;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float noiseThreshold = 0.45f;

    [Header("Detection")]
    [SerializeField] private float smoothing = 8f;

    private AudioClip microphoneClip;
    private string microphoneDevice;

    private bool microphoneReady = false;
    private bool detectionActive = false;

    private float currentNoise;

    private void Start()
    {
        if (noisePanel != null)
            noisePanel.SetActive(false);

        if (noiseSlider != null)
        {
            noiseSlider.minValue = 0f;
            noiseSlider.maxValue = 1f;
            noiseSlider.value = 0f;
        }

        StartMicrophone();
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No microphone detected!");
            return;
        }

        microphoneDevice = Microphone.devices[0];

        microphoneClip = Microphone.Start(
            microphoneDevice,
            true,
            1,
            44100
        );

        microphoneReady = microphoneClip != null;

        Debug.Log("Microphone started: " + microphoneDevice);
    }

    private void Update()
    {
        if (!microphoneReady || !Microphone.IsRecording(microphoneDevice))
            return;

        if (!detectionActive)
            return;

        float rawNoise = GetMicrophoneVolume();

        float targetNoise = Mathf.Clamp01(rawNoise * sensitivity);

        currentNoise = Mathf.Lerp(
            currentNoise,
            targetNoise,
            smoothing * Time.deltaTime
        );

        if (noiseSlider != null)
            noiseSlider.value = currentNoise;


        if (currentNoise >= noiseThreshold)
        {
            Debug.Log("THE WOLF HEARD YOU!");

            if (gameOverManager != null)
                gameOverManager.TriggerGameOver();
        }
    }

    private float GetMicrophoneVolume()
    {
        int position = Microphone.GetPosition(microphoneDevice);

        if (position < sampleWindow)
            return 0f;

        float[] samples = new float[sampleWindow];

        microphoneClip.GetData(
            samples,
            position - sampleWindow
        );

        float sum = 0f;

        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Mathf.Sqrt(sum / sampleWindow);
    }

    public void SetDetectionActive(bool active)
    {
        detectionActive = active;
        currentNoise = 0f;

        if (noisePanel != null)
            noisePanel.SetActive(active);

        if (noiseSlider != null)
            noiseSlider.value = 0f;

    }

    private void OnDestroy()
    {
        if (microphoneReady && Microphone.IsRecording(microphoneDevice))
        {
            Microphone.End(microphoneDevice);
        }
    }
}
