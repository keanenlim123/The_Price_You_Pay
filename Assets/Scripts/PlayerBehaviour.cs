/// <summary>
/// PlayerBehaviour.cs  
/// This script manages the player's interactions within the game world.  
/// It detects and processes player interactions with objects such as doors, shelves, mop buckets, candy piles, stolen items, friends, and more.  
/// It also handles task progress tracking (e.g., shelves lifted, footprints cleaned) and triggers cutscenes or scene transitions when objectives are met.  
/// Additional responsibilities include managing equipment (mop), dialogue sequences, respawning, and UI updates for interaction prompts and task progress.  
/// </summary>
/// <author> Keanen Lim Xi En </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10270417C </StudentID>
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{

    [Header("Interaction Settings")]
    /// <summary>
    /// Whether the player is currently able to interact with something.
    /// </summary>
    bool canInteract = false;

    /// <summary>
    /// Maximum range for interaction raycasts.
    /// </summary>
    [SerializeField] float interactRange = 2f;

    /// <summary>
    /// Vertical offset for the interaction raycast start point.
    /// </summary>
    [SerializeField] float rayHeightOffset = 1.0f;

    /// <summary>
    /// Current time the interact key has been held.
    /// </summary>
    float interactHoldTimer = 0f;

    /// <summary>
    /// Required hold time for certain actions.
    /// </summary>
    float maxHoldTime = 3f;

    /// <summary>
    /// UI text showing interaction timer progress.
    /// </summary>
    [SerializeField] private TextMeshProUGUI interactionTimerText;

    /// <summary>
    /// UI text showing interact prompt.
    /// </summary>
    [SerializeField] public TextMeshProUGUI Interact;


    [Header("Door & Teleport")]
    /// <summary>
    /// Reference to the door the player is currently looking at.
    /// </summary>
    DoorBehaviour currentDoor = null;

    /// <summary>
    /// Position to teleport to when respawning.
    /// </summary>
    public Transform teleport;


    [Header("Mop Settings")]
    /// <summary>
    /// Reference to the mop currently in the scene.
    /// </summary>
    BucketMop currentMop = null;

    /// <summary>
    /// Whether the player has picked up the mop.
    /// </summary>
    bool hasMop = false;

    /// <summary>
    /// Whether the mop is currently equipped.
    /// </summary>
    bool isMopEquipped = false;

    /// <summary>
    /// The mop visual GameObject.
    /// </summary>
    public GameObject mopVisual;

    /// <summary>
    /// Audio source played when mopping footprints.
    /// </summary>
    public AudioSource moppingAudio;

    /// <summary>
    /// UI icon or element representing the mop.
    /// </summary>
    public GameObject mopUI;


    [Header("Object References")]
    /// <summary>
    /// Reference to the shelf currently being interacted with.
    /// </summary>
    ShelfBehaviour currentShelf = null;

    /// <summary>
    /// Reference to the current footstep to clean.
    /// </summary>
    FootStepsBehaviour currentFootstep = null;

    /// <summary>
    /// Reference to the candy pile currently being searched.
    /// </summary>
    CandyBehaviour currentCandyPile = null;

    /// <summary>
    /// Whether the player currently has a stolen item.
    /// </summary>
    public bool hasStolenItem = false;

    /// <summary>
    /// Reference to the stolen item currently held.
    /// </summary>
    StolenItemBehaviour currentStolenItem = null;


    [Header("Friend Interaction")]
    /// <summary>
    /// Whether the player has already talked to the friend.
    /// </summary>
    bool isTalkedToFriend = false;

    /// <summary>
    /// Camera used for friend dialogue view.
    /// </summary>
    public Camera friendCamera;

    /// <summary>
    /// Canvas used for friend dialogue UI.
    /// </summary>
    public Canvas friendDialogueCanvas;

    /// <summary>
    /// Object to destroy during friend interaction sequence.
    /// </summary>
    public GameObject cubeToDestroy;

    /// <summary>
    /// Camera used for store clerk cutscene.
    /// </summary>
    public Camera storeClerkCamera;

    /// <summary>
    /// General dialogue UI canvas.
    /// </summary>
    public Canvas dialogueCanvas;

    /// <summary>
    /// Whether the friend interaction sequence has started.
    /// </summary>
    private bool sequenceStarted = false;

    /// <summary>
    /// Reference to the current friend being interacted with.
    /// </summary>
    FriendBehaviour currentFriend = null;


    [Header("Task Tracking")]
    /// <summary>
    /// UI text for the main task description.
    /// </summary>
    [SerializeField] public TextMeshProUGUI Task;

    /// <summary>
    /// UI text for the secondary task description.
    /// </summary>
    [SerializeField] public TextMeshProUGUI Task2;

    /// <summary>
    /// Number of shelves the player has lifted.
    /// </summary>
    public int shelvesLiftedCount = 0;

    /// <summary>
    /// Number of footprints the player has cleaned.
    /// </summary>
    public int footprintsCleanedCount = 0;


    [Header("Cutscene & UI Fade")]
    /// <summary>
    /// Camera used for cutscenes.
    /// </summary>
    public GameObject cameraGameObject;

    /// <summary>
    /// Canvas containing the fade animation.
    /// </summary>
    public Canvas animatorCanvas;

    /// <summary>
    /// Duration of fade transitions in seconds.
    /// </summary>
    public float fadeDuration = 2f;

    /// <summary>
    /// Called once per frame to handle raycasting and interaction checks.
    /// </summary>
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

    /// <summary>
    /// Triggers the store clerk cutscene sequence when the player tries to leave after stealing.
    /// </summary>
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

    /// <summary>
    /// Teleports the player back to the spawn/teleport point.
    /// Resets shelves lifted UI progress.
    /// </summary>
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

    /// <summary>
    /// Handles immediate interaction actions (e.g., opening doors, picking up mop).
    /// </summary>
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
                mopUI.SetActive(true);
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

    /// <summary>
    /// Toggles the mop's equipped state.
    /// </summary>
    void OnEquip()
    {
        if (hasMop)
        {
            isMopEquipped = !isMopEquipped;
            mopVisual.SetActive(isMopEquipped);
        }
    }

    /// <summary>
    /// Starts the conversation sequence with the friend.
    /// </summary>
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

    /// <summary>
    /// Ends the conversation sequence with the friend.
    /// </summary>
    void EndFriendConversation()
    {
        isTalkedToFriend = false;

        if (friendCamera != null)
            friendCamera.gameObject.SetActive(false);

        if (friendDialogueCanvas != null)
            friendDialogueCanvas.gameObject.SetActive(false);

        Debug.Log("Friend conversation ended");
    }

    /// <summary>
    /// Handles hold-to-interact mechanics for shelves, candy piles, and footprints.
    /// </summary>
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
                if (moppingAudio != null && !moppingAudio.isPlaying)
                {
                    moppingAudio.Play();
                }


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

    /// <summary>
    /// Resets the shelves lifted counter and updates UI.
    /// </summary>
    public void ResetShelvesLiftedCount()
    {
        shelvesLiftedCount = 0;
        Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";
        UpdateShelvesUI();
    }

    /// <summary>
    /// Decreases the shelves lifted counter by one and updates UI.
    /// </summary>
    public void DecreaseShelvesLiftedCount()
    {
        shelvesLiftedCount = Mathf.Max(0, shelvesLiftedCount - 1);
        Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";
        UpdateShelvesUI();
    }

    /// <summary>
    /// Decreases the footprints cleaned counter by one and updates UI.
    /// </summary>
    public void DecreaseFootprintsCleanedCount()
    {
        footprintsCleanedCount = Mathf.Max(0, footprintsCleanedCount - 1);
        Task.text = $"- Clean footprints {footprintsCleanedCount} / 10";
        UpdateFootprintsUI();
    }

    /// <summary>
    /// Updates the footprints cleaned UI text.
    /// </summary>
    public void UpdateFootprintsUI()
    {
        if (footprintsCleanedCount >= 10)
            Task.text = $"<s>- Clean footprints {footprintsCleanedCount} / 10</s>";
        else
            Task.text = $"- Clean footprints {footprintsCleanedCount} / 10";

        CheckAllTasksCompleted();
    }

    /// <summary>
    /// Updates the shelves lifted UI text.
    /// </summary>
    public void UpdateShelvesUI()
    {
        if (shelvesLiftedCount >= 10)
            Task2.text = $"<s>- Shelves lifted: {shelvesLiftedCount} / 10</s>";
        else
            Task2.text = $"- Shelves lifted: {shelvesLiftedCount} / 10";

        CheckAllTasksCompleted();
    }

    /// <summary>
    /// Checks if all game objectives are completed, and if so, starts the final cutscene.
    /// </summary>
    void CheckAllTasksCompleted()
    {
        if (shelvesLiftedCount >= 10 && footprintsCleanedCount >= 10 && CandyBehaviour.candyBarFound)
        {
            Debug.Log("All tasks completed! Starting coroutine.");
            StartCoroutine(CompleteGameSequence());
        }
    }

    /// <summary>
    /// Final sequence coroutine that plays a cutscene and returns to main menu.
    /// </summary>
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
        yield return new WaitForSeconds(80f);

        // 5. Load Main Menu scene
        SceneManager.LoadScene(0);
    }
}