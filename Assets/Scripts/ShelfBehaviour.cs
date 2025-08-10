using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    public GameObject upShelf;
    public GameObject downShelf;

    private bool isKnockedDown = false;

    // Declare an event for shelf knockdown
    public delegate void ShelfKnockedDownHandler(ShelfBehaviour shelf);
    public static event ShelfKnockedDownHandler OnShelfKnockedDown;

    void Start()
    {
        isKnockedDown = downShelf.activeSelf;
    }

    public void KnockDown()
    {
        if (isKnockedDown) return;

        isKnockedDown = true;
        Debug.Log("KnockDown: Setting upShelf = false, downShelf = true");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(false);
            downShelf.SetActive(true);
        }
        else
        {
            Debug.LogError("Shelf visuals not assigned!");
        }

        // Fire event to notify listeners (like player)
        OnShelfKnockedDown?.Invoke(this);
    }

    public void LiftShelf()
    {
        if (!isKnockedDown)
        {
            Debug.Log($"{gameObject.name} is already lifted.");
            return;
        }

        isKnockedDown = false;
        Debug.Log($"{gameObject.name} LiftShelf called! upShelf: {upShelf}, downShelf: {downShelf}");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(true);
            downShelf.SetActive(false);
            Debug.Log("Shelf visuals updated: upShelf active, downShelf inactive.");
        }
        else
        {
            Debug.LogError("Shelf visuals not assigned!");
        }
    }

    public bool IsKnockedDown()
    {
        return isKnockedDown;
    }
}
