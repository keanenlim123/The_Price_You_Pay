using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    public GameObject upShelf;
    public GameObject downShelf;

    private bool isKnockedDown = false;

    private PlayerBehaviour playerBehaviour;

    private float knockCooldown = 1f;
    private float knockTimer = 0f;

    void Start()
    {
        isKnockedDown = downShelf.activeSelf;

        // Find the player and cache the PlayerBehaviour reference (assuming player tagged "Player")
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
        if (isKnockedDown) return;

        isKnockedDown = true;
        Debug.Log($"[KnockDown] Shelf: {gameObject.name}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(false);
            downShelf.SetActive(true);
        }

        // Tell player to decrease shelf count by 1
        if (playerBehaviour != null)
        {
            playerBehaviour.DecreaseShelvesLiftedCount();
        }
    }

    public void LiftShelf()
    {
        if (!isKnockedDown) return;

        isKnockedDown = false;
        Debug.Log($"[LiftShelf] Shelf: {gameObject.name}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(true);
            downShelf.SetActive(false);
        }
    }

    public bool IsKnockedDown()
    {
        return isKnockedDown;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Greed"))
        {
            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0f)
            {
                KnockDown();
                knockTimer = knockCooldown;
            }
        }
    }
}
