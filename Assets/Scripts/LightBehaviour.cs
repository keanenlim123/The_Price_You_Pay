using UnityEngine;

public class LightsBehaviour : MonoBehaviour
{
   public Light flickerLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 5f;
    public float flickerSpeed = 0.1f;

    private float timer;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();
    }

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
