using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    public GameObject upShelf;
    public GameObject downShelf;

    private bool isKnockedDown = false;

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
    }

    public void LiftShelf()
    {
        if (!isKnockedDown) return;

        isKnockedDown = false;
        Debug.Log("LiftShelf: Setting upShelf = true, downShelf = false");

        if (upShelf != null && downShelf != null)
        {
            upShelf.SetActive(true);
            downShelf.SetActive(false);
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
