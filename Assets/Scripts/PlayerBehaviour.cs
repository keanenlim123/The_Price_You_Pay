using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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

    public bool hasStolenItem = false;
    StolenItemBehaviour currentStolenItem = null;

    bool isTalkedToFriend = false;
    public Camera friendCamera;
    public Canvas friendDialogueCanvas;
    public GameObject cubeToDestroy;
    public Camera storeClerkCamera;

    public Canvas dialogueCanvas;
    private bool sequenceStarted = false;
    void Update()
    {
        RaycastHit hit;
        canInteract = false;
        currentDoor = null;
        currentMop = null;
        currentShelf = null;
        currentFootstep = null;
        currentCandyPile = null;
        currentStolenItem = null;

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
            else if (hitObject.CompareTag("StolenItem") && !hasStolenItem)
            {
                canInteract = true;
                currentStolenItem = hitObject.GetComponent<StolenItemBehaviour>();
            }
            else if (hasStolenItem && !sequenceStarted && hit.collider.CompareTag("SlideDoor"))
            {
                StartCoroutine(StoreClerkSequence());
            }
            else if (hitObject.CompareTag("Friend")) 
            {
                canInteract = true;
                Debug.Log("Looking at friend, press interact to talk");
            }
            Debug.DrawRay(rayOrigin, transform.forward * interactRange, Color.green);
        }

        HandleHoldInteraction();
    }
    private IEnumerator StoreClerkSequence()
    {
        sequenceStarted = true;


        storeClerkCamera.gameObject.SetActive(true);
        storeClerkCamera.enabled = true;

        // Show dialogue
        if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(true);

        // Wait before loading next scene
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                currentDoor.OpenDoor();
            }
            else if (currentMop != null)
            {
                currentMop.Collect(this);
                hasMop = true;
            }
            else if (currentStolenItem != null && !hasStolenItem)
            {
                currentStolenItem.Steal(this);
            }
            else if (!isTalkedToFriend)
            {
                StartFriendConversation();
            }
            else if (isTalkedToFriend)
            {
                EndFriendConversation();
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
    void StartFriendConversation()
    {
        if (isTalkedToFriend) return;

        isTalkedToFriend = true;

        // Show friend camera and dialogue UI
        if (friendCamera != null)
            friendCamera.gameObject.SetActive(true);

        if (friendDialogueCanvas != null)
            friendDialogueCanvas.gameObject.SetActive(true);

        // Destroy the cube
        if (cubeToDestroy != null)
            Destroy(cubeToDestroy);

        Debug.Log("Started conversation with friend");
    }

    void EndFriendConversation()
    {
        isTalkedToFriend = false;

        if (friendCamera != null)
            friendCamera.gameObject.SetActive(false);

        if (friendDialogueCanvas != null)
            friendDialogueCanvas.gameObject.SetActive(false);

        Debug.Log("Friend conversation ended");
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