using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    public GameObject upShelf;
    public GameObject downShelf;

    public AudioSource pushUpAudio; // Audio for pushing shelf back up

    private bool isKnockedDown = false;
    private int liftCount = 0;             // How many times shelf lifted
    private bool knockdownAllowed = true; // Disable knockdown after 2 lifts
    private bool hasAddedPointThisLift = false;

    // Track if this shelf currently counts towards the player's lifted shelf count
    private bool hasCountedAsLifted = false;

    public PlayerBehaviour playerBehaviour;

    void Start()
    {
        // Set initial state based on downShelf active state
        isKnockedDown = downShelf.activeSelf;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerBehaviour = playerObj.GetComponent<PlayerBehaviour>();
            if (playerBehaviour == null)
                Debug.LogWarning("PlayerBehaviour script not found on Player object");
        }
        else
        {
            Debug.LogWarning("Player object not found in scene");
        }
    }

    public void KnockDown()
    {
        if (!knockdownAllowed)
        {
            Debug.Log("[KnockDown] Knockdown disallowed after 2 lifts.");
            return;
        }

        if (isKnockedDown) return;

        if (playerBehaviour != null && playerBehaviour.shelvesLiftedCount >= 10)
            return;

        isKnockedDown = true;
        hasAddedPointThisLift = false; // Reset flag when knocked down

        Debug.Log($"[KnockDown] Shelf: {gameObject.name}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(false);
            downShelf.SetActive(true);
        }

        // Only decrease count if this shelf was previously counted as lifted
        if (playerBehaviour != null && hasCountedAsLifted)
        {
            playerBehaviour.DecreaseShelvesLiftedCount();
            hasCountedAsLifted = false;  // Mark that this shelf no longer counts as lifted
        }
    }

    public void LiftShelf()
    {
        if (!isKnockedDown) return; // Only lift if knocked down

        isKnockedDown = false;
        liftCount++;

        Debug.Log($"[LiftShelf] Shelf: {gameObject.name}, liftCount: {liftCount}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(true);
            downShelf.SetActive(false);
        }

        // Play push-up sound
        if (pushUpAudio != null && !pushUpAudio.isPlaying)
        {
            pushUpAudio.Play();
        }

        // Only add to player's lifted count once per lift
        if (playerBehaviour != null && !hasAddedPointThisLift)
        {
            if (liftCount <= 2)
            {
                playerBehaviour.shelvesLiftedCount++;
                playerBehaviour.UpdateShelvesUI();
                hasAddedPointThisLift = true;  // Mark that we've added point for this lift
                hasCountedAsLifted = true;     // Mark that this shelf counts towards player's total
            }
        }

        if (liftCount >= 2)
        {
            knockdownAllowed = false; // Disable knockdown after second lift
            Debug.Log("[LiftShelf] Knockdown disabled after second lift.");
        }
    }

    public bool IsKnockedDown()
    {
        return isKnockedDown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Greed"))
        {
            KnockDown();
        }
    }
}
