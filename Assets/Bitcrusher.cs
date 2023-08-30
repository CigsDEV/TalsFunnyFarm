using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bitcrusher : MonoBehaviour
{
    [Tooltip("Set the bit depth, lower values result in a more pronounced bitcrushing effect.")]
    [Range(1, 24)]
    public int bitDepth = 8;

    private float maxSample;
    private float stepSize;

    private void Start()
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        maxSample = Mathf.Pow(2, bitDepth - 1);
        stepSize = 1.0f / maxSample;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Clamp(Mathf.Round(data[i] / stepSize) * stepSize, -1, 1);
        }
    }

    // Ensure that any changes to bitDepth in the Inspector are immediately reflected.
    private void OnValidate()
    {
        UpdateValues();
    }
}
