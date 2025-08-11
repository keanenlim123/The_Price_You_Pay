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

    FriendBehaviour currentFriend = null;

    [SerializeField]
    public TextMeshProUGUI Task;
    [SerializeField]
    public TextMeshProUGUI Task2;

    [SerializeField]
    public TextMeshProUGUI Interact;

    [SerializeField]
    private TextMeshProUGUI interactionTimerText;

    public int shelvesLiftedCount = 0;
    public int footprintsCleanedCount = 0;

    [Header("Cutscene & UI Fade")]
    public GameObject cameraGameObject;
    public Canvas animatorCanvas;
    public float fadeDuration = 2f;



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
                Interact.gameObject.SetActive(true);
                Interact.text = "Press E to Open Door";
            }
            else if (hitObject.CompareTag("BucketMop"))
            {
                canInteract = true;
                currentMop = hitObject.GetComponent<BucketMop>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Press E to Collect Mop";
            }
            else if (hitObject.CompareTag("Shelf"))
            {
                canInteract = true;
                currentShelf = hitObject.GetComponent<ShelfBehaviour>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Hold E to Lift Shelf";
            }
            else if (hitObject.CompareTag("Footstep") && isMopEquipped)
            {
                canInteract = true;
                currentFootstep = hitObject.GetComponent<FootStepsBehaviour>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Hold E to Clean Footstep";
            }
            else if (hitObject.CompareTag("CandyPile"))
            {
                canInteract = true;
                currentCandyPile = hitObject.GetComponent<CandyBehaviour>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Hold E to Search Pile";
            }
            else if (hitObject.CompareTag("StolenItem") && !hasStolenItem)
            {
                canInteract = true;
                currentStolenItem = hitObject.GetComponent<StolenItemBehaviour>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Press E to steal " + hitObject.name;
            }
            else if (hasStolenItem && !sequenceStarted && hit.collider.CompareTag("SlideDoor"))
            {
                StartCoroutine(StoreClerkSequence());
                Interact.text = "";
            }
            else if (hitObject.CompareTag("Friend"))
            {
                canInteract = true;
                currentFriend = hitObject.GetComponent<FriendBehaviour>();
                Interact.gameObject.SetActive(true);
                Interact.text = "Press E to Talk";
            }
            else if (hitObject.CompareTag("Wall"))
            {
                canInteract = true;
                Interact.gameObject.SetActive(true);
                Interact.text = "Talk with your friend first";
            }
            else
            {
                Interact.text = "";
            }

            Debug.DrawRay(rayOrigin, transform.forward * interactRange, Color.green);
        }
        else
        {
            Interact.text = "";
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
            Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";
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
                Task.text = "- Clean footprints 0 / 10";
            }
            else if (currentStolenItem != null && !hasStolenItem)
            {
                currentStolenItem.Steal(this);
                Task.text = "- Meet up with your friend";
            }
            else if (currentFriend != null)
            {
                if (!isTalkedToFriend)
                    StartFriendConversation();
                else
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
        Interact.gameObject.SetActive(false);
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

        if (Task != null)
            Task.text = "- Enter the store and steal something";

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
        if (canInteract && Input.GetKey(KeyCode.E))
        {
            bool isHolding = false;

            if (currentShelf != null)
            {
                isHolding = true;
                interactHoldTimer += Time.deltaTime;
                float timeLeft = Mathf.Max(0f, maxHoldTime - interactHoldTimer);
                interactionTimerText.gameObject.SetActive(true);
                interactionTimerText.text = $"Hold... {timeLeft:F1}s";

                if (interactHoldTimer >= maxHoldTime)
                {
                    if (currentShelf.IsKnockedDown())
                    {
                        currentShelf.LiftShelf();
                        UpdateShelvesUI();
                    }
                    interactHoldTimer = 0f;
                    interactionTimerText.gameObject.SetActive(false);
                }
            }
            else if (currentCandyPile != null)
            {
                isHolding = true;
                interactHoldTimer += Time.deltaTime;
                float timeLeft = Mathf.Max(0f, maxHoldTime - interactHoldTimer);
                interactionTimerText.gameObject.SetActive(true);
                interactionTimerText.text = $"Searching... {timeLeft:F1}s";

                if (interactHoldTimer >= maxHoldTime)
                {
                    currentCandyPile.SearchPile();
                    interactHoldTimer = 0f;
                    interactionTimerText.gameObject.SetActive(false);
                }
            }
            else if (currentFootstep != null && isMopEquipped)
            {
                isHolding = true;
                interactHoldTimer += Time.deltaTime;
                float timeLeft = Mathf.Max(0f, maxHoldTime - interactHoldTimer);
                interactionTimerText.gameObject.SetActive(true);
                interactionTimerText.text = $"Cleaning... {timeLeft:F1}s";

                if (interactHoldTimer >= maxHoldTime)
                {
                    currentFootstep.Clean();
                    footprintsCleanedCount++;
                    UpdateFootprintsUI();
                    interactHoldTimer = 0f;
                    interactionTimerText.gameObject.SetActive(false);
                }
            }

            if (!isHolding)
            {
                interactHoldTimer = 0f;
                interactionTimerText.gameObject.SetActive(false);
            }
        }
        else
        {
            interactHoldTimer = 0f;
            interactionTimerText.gameObject.SetActive(false);
        }
    }
    public void ResetShelvesLiftedCount()
    {
        shelvesLiftedCount = 0;
        Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";
        UpdateShelvesUI();
    }

    public void DecreaseShelvesLiftedCount()
    {
        shelvesLiftedCount = Mathf.Max(0, shelvesLiftedCount - 1);
        Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";
        UpdateShelvesUI();
    }
    public void DecreaseFootprintsCleanedCount()
    {
        footprintsCleanedCount = Mathf.Max(0, footprintsCleanedCount - 1);
        Task.text = $"- Clean footprints {footprintsCleanedCount} / 10";
        UpdateFootprintsUI();
    }
    public void UpdateFootprintsUI()
    {
        if (footprintsCleanedCount >= 10)
            Task.text = $"<s>- Clean footprints {footprintsCleanedCount} / 10</s>";
        else
            Task.text = $"- Clean footprints {footprintsCleanedCount} / 10";

        CheckAllTasksCompleted();
    }

    public void UpdateShelvesUI()
    {
        if (shelvesLiftedCount >= 10)
            Task2.text = $"<s>- Shelves lifted: {shelvesLiftedCount} / 10</s>";
        else
            Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";

        CheckAllTasksCompleted();
    }


    void CheckAllTasksCompleted()
    {
        if (shelvesLiftedCount >= 10 && footprintsCleanedCount >= 10 && CandyBehaviour.candyBarFound)
        {
            Debug.Log("All tasks completed! Starting coroutine.");
            StartCoroutine(CompleteGameSequence());
        }
    }


    private IEnumerator CompleteGameSequence()
    {
        // 1. Show camera GameObject
        cameraGameObject.SetActive(true);

        // 2. Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // 3. Hide camera GameObject and show Canvas with Animator
        cameraGameObject.SetActive(false);

        animatorCanvas.gameObject.SetActive(true);

        // Optionally trigger animator if needed
        Animator animator = animatorCanvas.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("YourAnimationStateName"); // replace with your animation state name
        }

        // 4. Wait 43 seconds
        yield return new WaitForSeconds(43f);

        // 5. Load Main Menu scene (assumed index 0)
        SceneManager.LoadScene(0);
    }
}