/// <summary>
/// ShelfBehaviour.cs
/// This script controls the behaviour of fallen shelves, allowing them to be knocked down
/// and lifted by the player. It also tracks lift counts and prevents further knockdowns
/// after two successful lifts.
/// </summary>
/// <author> Lim Xue Zhi Conan </author>
/// <date> 11/8/2025 </date>
/// <Student ID> S10269214H </Student ID>

using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    ///<summary>
    /// Reference to the upright shelf GameObject.
    ///</summary>
    public GameObject upShelf;

    ///<summary>
    /// Reference to the knocked-down shelf GameObject.
    ///</summary>
    public GameObject downShelf;

    ///<summary>
    /// Audio played when the shelf is pushed back up.
    ///</summary>
    public AudioSource pushUpAudio;

    ///<summary>
    /// Whether the shelf is currently knocked down.
    ///</summary>
    private bool isKnockedDown = false;

    ///<summary>
    /// The number of times the shelf has been lifted.
    ///</summary>
    private int liftCount = 0;

    ///<summary>
    /// Whether the shelf can still be knocked down (disabled after 2 lifts).
    ///</summary>
    private bool knockdownAllowed = true;

    ///<summary>
    /// Tracks if a point has been added for this lift.
    ///</summary>
    private bool hasAddedPointThisLift = false;

    ///<summary>
    /// Tracks whether this shelf currently counts towards the player's lifted shelf total.
    ///</summary>
    private bool hasCountedAsLifted = false;

    ///<summary>
    /// Reference to the PlayerBehaviour script for updating player stats.
    ///</summary>
    public PlayerBehaviour playerBehaviour;

    ///<summary>
    /// Initializes the shelf state and finds the PlayerBehaviour reference.
    ///</summary>
    void Start()
    {
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

    ///<summary>
    /// Knocks the shelf down if allowed, updating its state and player's lift count.
    ///</summary>
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
        hasAddedPointThisLift = false;

        Debug.Log($"[KnockDown] Shelf: {gameObject.name}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(false);
            downShelf.SetActive(true);
        }

        if (playerBehaviour != null && hasCountedAsLifted)
        {
            playerBehaviour.DecreaseShelvesLiftedCount();
            hasCountedAsLifted = false;
        }
    }

    ///<summary>
    /// Lifts the shelf back up, plays audio, and updates player stats.
    ///</summary>
    public void LiftShelf()
    {
        if (!isKnockedDown) return;

        isKnockedDown = false;
        liftCount++;

        Debug.Log($"[LiftShelf] Shelf: {gameObject.name}, liftCount: {liftCount}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(true);
            downShelf.SetActive(false);
        }

        if (pushUpAudio != null && !pushUpAudio.isPlaying)
        {
            pushUpAudio.Play();
        }

        if (playerBehaviour != null && !hasAddedPointThisLift)
        {
            if (liftCount <= 2)
            {
                playerBehaviour.shelvesLiftedCount++;
                playerBehaviour.UpdateShelvesUI();
                hasAddedPointThisLift = true;
                hasCountedAsLifted = true;
            }
        }

        if (liftCount >= 2)
        {
            knockdownAllowed = false;
            Debug.Log("[LiftShelf] Knockdown disabled after second lift.");
        }
    }

    ///<summary>
    /// Returns whether the shelf is currently knocked down.
    ///</summary>
    public bool IsKnockedDown()
    {
        return isKnockedDown;
    }

    ///<summary>
    /// Detects collisions with the Greed enemy to knock the shelf down.
    ///</summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Greed"))
        {
            KnockDown();
        }
    }
}
