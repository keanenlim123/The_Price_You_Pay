using UnityEngine;

public class StolenItemBehaviour : MonoBehaviour

{
    public AudioSource pickupAudio;
    public void Steal(PlayerBehaviour player)
    {
        // Mark the player as having stolen an item
        player.hasStolenItem = true;


        // Destroy the stolen item from the scene
        Destroy(gameObject);

        Debug.Log("Item stolen!");
        if (pickupAudio != null && !pickupAudio.isPlaying)
        {
            pickupAudio.Play();
        }
    }
}
