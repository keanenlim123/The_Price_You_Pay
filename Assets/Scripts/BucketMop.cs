using UnityEngine;

public class BucketMop : MonoBehaviour

{
    public AudioSource pickupAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Collect(PlayerBehaviour player)
    {
        if (pickupAudio != null && !pickupAudio.isPlaying)
        {
            pickupAudio.Play();
        }
        Destroy(gameObject);
    }
}
