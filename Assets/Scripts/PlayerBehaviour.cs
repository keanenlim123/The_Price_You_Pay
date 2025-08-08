using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    bool canInteract = false;
    DoorBehaviour currentDoor = null;
    public Transform teleport;

    [SerializeField]
    float interactRange = 2f;
    [SerializeField]
    float rayHeightOffset = 1.0f;

    BucketMop currentMop = null;

    bool hasMop = false;
    bool isMopEquipped = false;
    public GameObject mopVisual;

    ShelfBehaviour currentShelf = null;

    float interactHoldTimer = 0f;
    float maxHoldTime = 3f;

    FootStepsBehaviour currentFootstep = null;

    CandyBehaviour currentCandyPile = null;


    void Update()
    {
        RaycastHit hit;
        canInteract = false;
        currentDoor = null;
        currentMop = null;
        currentShelf = null;
        currentFootstep = null;
        currentCandyPile = null;

        Vector3 rayOrigin = transform.position + Vector3.up * rayHeightOffset;
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, interactRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Door"))
            {
                canInteract = true;
                currentDoor = hitObject.GetComponent<DoorBehaviour>();
            }
            else if (hitObject.CompareTag("BucketMop"))
            {
                canInteract = true;
                currentMop = hitObject.GetComponent<BucketMop>();
            }
            else if (hitObject.CompareTag("Shelf"))
            {
                Debug.Log("Shelf");
                canInteract = true;
                currentShelf = hitObject.GetComponent<ShelfBehaviour>();
            }
            else if (hitObject.CompareTag("Footstep") && isMopEquipped)
            {
                canInteract = true;
                currentFootstep = hitObject.GetComponent<FootStepsBehaviour>();
            }
            else if (hitObject.CompareTag("CandyPile"))
            {
                canInteract = true;
                currentCandyPile = hitObject.GetComponent<CandyBehaviour>();
            }


            Debug.DrawRay(rayOrigin, transform.forward * interactRange, Color.green);
        }

        HandleHoldInteraction(); // NEW FUNCTION
    }


    public void Respawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (teleport != null)
        {
            transform.position = teleport.position;
            Debug.Log("Teleporting to: " + teleport.position);

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }

            Physics.SyncTransforms();
        }
        else
        {
            Debug.LogWarning("Spawn location not assigned!");
        }

    }
    void OnInteract()
    {
        if (canInteract)
        {
            if (currentDoor != null)
            {
                Debug.Log("Interacting with Door2");
                currentDoor.OpenDoor();
            }
            else if (currentMop != null)
            {
                currentMop.Collect(this);
                hasMop = true;
            }
        }
    }

    void OnEquip()
    {
        if (hasMop)
        {
            isMopEquipped = !isMopEquipped;
            mopVisual.SetActive(isMopEquipped);
        }
    }

    void HandleHoldInteraction()
    {
        if (canInteract)
        {
            if (currentShelf != null)
            {
                interactHoldTimer += Time.deltaTime;

                if (interactHoldTimer >= maxHoldTime)
                {
                    if (currentShelf.IsKnockedDown())
                    {
                        currentShelf.LiftShelf();
                    }

                    interactHoldTimer = 0f;
                }
            }
            else if (currentCandyPile != null)
            {
                interactHoldTimer += Time.deltaTime;

                if (interactHoldTimer >= maxHoldTime)
                {
                    currentCandyPile.SearchPile();
                    interactHoldTimer = 0f;
                }
            }
            else if (currentFootstep != null && isMopEquipped)
            {
                interactHoldTimer += Time.deltaTime;

                if (interactHoldTimer >= maxHoldTime)
                {
                    currentFootstep.Clean();
                    interactHoldTimer = 0f;
                }
            }
            else
            {
                interactHoldTimer = 0f;
            }
        }
        else
        {
            interactHoldTimer = 0f;
        }
    }
}