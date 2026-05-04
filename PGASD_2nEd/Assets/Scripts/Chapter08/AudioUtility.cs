using UnityEngine;

/// <summary>
/// The AudioUtility class provides helper functions for frequent audio operations
/// </summary>
public class AudioUtility
{
    /// <summary>
    /// Find the linear volume value in range 0 and 1 for a given perceptual loudness.
    /// </summary>
    /// <param name="loudness">Perceived loudness to be mapped to linear volume.</param>
    /// <param name="noiseFloor">Reference value corresponding to silence.</param>
    /// <returns>Corresponding linear volume for the given perceived loudness.</returns>
    public static float LoudnessToLinearVolume(float loudness, float noiseFloor = -80.0f)
    {
        // Ensure value is between 0 and 1
        float clampedLoudness = Mathf.Clamp01(loudness);
        
        // Linearly interpolate clamped value between -80.0f and 0.0f
        float dB = Mathf.Lerp(noiseFloor, 0.0f, clampedLoudness);
        
        // Bring the value for dB back to linear scale
        float linearVolume = Mathf.Pow(10.0f, dB / 20.0f);
        
        return linearVolume;
    }

    /// <summary>
    /// Find the corresponding loudness in dB for a given linear volume.
    /// </summary>
    /// <param name="linearVolume">Linear volume between 0.0001 and 1.</param>
    /// <returns>Corresponding loudness in dB for the provided linear volume.</returns>
    public static float LinearVolumeToLoudness(float linearVolume)
    {
        // Clamp volume to avoid calculating the log of 0
        float clampedVolume = Mathf.Clamp(linearVolume, 0.0001f, 1.0f);
        
        // Convert volume to loudness
        float dB = 20.0f * Mathf.Log10(clampedVolume);
        
        return dB;
    }
}
