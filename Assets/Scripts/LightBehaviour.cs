/// <summary>
/// LightBehaviour.cs
/// Handles the flickering effect of a light in the scene
/// by randomly adjusting its intensity over time.
/// </summary>
/// <author>Chia Jia Cong Justin</author>
/// <date>5/8/2025</date>
/// <StudentID>S10266690C</StudentID>
using UnityEngine;

public class LightsBehaviour : MonoBehaviour
{
    /// <summary>
    /// The light component to apply the flicker effect to.
    /// If left null, it will try to use the Light component
    /// on the same GameObject.
    /// </summary>
    public Light flickerLight;

    /// <summary>
    /// The minimum intensity the light can flicker to.
    /// </summary>
    public float minIntensity = 0.5f;

    /// <summary>
    /// The maximum intensity the light can flicker to.
    /// </summary>
    public float maxIntensity = 5f;

    /// <summary>
    /// How often (in seconds) the light's intensity changes.
    /// Smaller values result in faster flickering.
    /// </summary>
    public float flickerSpeed = 0.1f;

    /// <summary>
    /// Internal timer to track when to change intensity.
    /// </summary>
    private float timer;

    /// <summary>
    /// Initializes the light reference if not already set.
    /// </summary>

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();
    }

    /// <summary>
    /// Updates the light intensity at intervals based on <see cref="flickerSpeed"/>.
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= flickerSpeed)
        {
            flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = 0f;
        }
    }
}
