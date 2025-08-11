using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    public GameObject upShelf;
    public GameObject downShelf;

    private bool isKnockedDown = false;
    private bool hasBeenKnockedByGreed = false;

    private PlayerBehaviour playerBehaviour;

    private float knockCooldown = 1f;
    private float knockTimer = 0f;

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

    public void KnockDown()
    {
        if (isKnockedDown) return;

        if (hasBeenKnockedByGreed) return;

        if (playerBehaviour != null && playerBehaviour.shelvesLiftedCount >= 10)
            return;

        isKnockedDown = true;
        hasBeenKnockedByGreed = true;

        Debug.Log($"[KnockDown] Shelf: {gameObject.name}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(false);
            downShelf.SetActive(true);
        }

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
            if (playerBehaviour != null && playerBehaviour.shelvesLiftedCount >= 10)
                return;

            if (hasBeenKnockedByGreed) return;

            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0f)
            {
                KnockDown();
                knockTimer = knockCooldown;
            }
        }
    }
}
