///<summary>
/// PlayerBehaviour.cs  
/// This script manages the player's interactions within the game world.  
/// It detects and processes player interactions with objects such as doors, shelves, mop buckets, candy piles, stolen items, friends, and more.  
/// It also handles task progress tracking (e.g., shelves lifted, footprints cleaned) and triggers cutscenes or scene transitions when objectives are met.  
/// Additional responsibilities include managing equipment (mop), dialogue sequences, respawning, and UI updates for interaction prompts and task progress.  
///</summary>
///<author> Keanen Lim Xi En </author>
///<date> 4/8/2025 </date>
///<StudentID> S10269214H </StudentID>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    ///<summary>Flag indicating if the player can interact with an object.</summary>
    bool canInteract = false;

    ///<summary>The current door the player is interacting with.</summary>
    DoorBehaviour currentDoor = null;

    ///<summary>Teleport location for respawning the player.</summary>
    public Transform teleport;

    [SerializeField] ///<summary>Interaction detection range in meters.</summary>
    float interactRange = 2f;

    [SerializeField] ///<summary>Vertical offset for raycasting from the player.</summary>
    float rayHeightOffset = 1.0f;

    ///<summary>The current mop bucket the player can interact with.</summary>
    BucketMop currentMop = null;

    ///<summary>Flag indicating if the player has collected the mop.</summary>
    bool hasMop = false;

    ///<summary>Flag indicating if the mop is currently equipped.</summary>
    bool isMopEquipped = false;

    ///<summary>Visual representation of the mop.</summary>
    public GameObject mopVisual;

    ///<summary>Audio source for mopping sound effects.</summary>
    public AudioSource moppingAudio;

    ///<summary>The current shelf the player can interact with.</summary>
    ShelfBehaviour currentShelf = null;

    ///<summary>Timer tracking how long the interaction button has been held.</summary>
    float interactHoldTimer = 0f;

    ///<summary>Maximum time required to complete a hold interaction.</summary>
    float maxHoldTime = 3f;

    ///<summary>The current footstep the player can clean.</summary>
    FootStepsBehaviour currentFootstep = null;

    ///<summary>The current candy pile the player can search.</summary>
    CandyBehaviour currentCandyPile = null;

    ///<summary>Flag indicating if the player has stolen an item.</summary>
    public bool hasStolenItem = false;

    ///<summary>The current stolen item the player can pick up.</summary>
    StolenItemBehaviour currentStolenItem = null;

    ///<summary>Flag indicating if the player has talked to their friend.</summary>
    bool isTalkedToFriend = false;

    ///<summary>Camera used for friend dialogue sequences.</summary>
    public Camera friendCamera;

    ///<summary>UI canvas for friend dialogue.</summary>
    public Canvas friendDialogueCanvas;

    ///<summary>GameObject to destroy after friend conversation.</summary>
    public GameObject cubeToDestroy;

    ///<summary>Camera used for store clerk interaction sequence.</summary>
    public Camera storeClerkCamera;

    ///<summary>UI canvas for store clerk dialogue.</summary>
    public Canvas dialogueCanvas;

    ///<summary>Flag to ensure certain sequences start only once.</summary>
    private bool sequenceStarted = false;

    ///<summary>The current friend NPC the player can interact with.</summary>
    FriendBehaviour currentFriend = null;

    [SerializeField] ///<summary>UI text for displaying the main task.</summary>
    public TextMeshProUGUI Task;

    [SerializeField] ///<summary>UI text for displaying the secondary task.</summary>
    public TextMeshProUGUI Task2;

    [SerializeField] ///<summary>UI text for displaying interaction prompts.</summary>
    public TextMeshProUGUI Interact;

    [SerializeField] ///<summary>UI text for showing hold interaction progress.</summary>
    private TextMeshProUGUI interactionTimerText;

    ///<summary>Tracks the number of shelves lifted by the player.</summary>
    public int shelvesLiftedCount = 0;

    ///<summary>Tracks the number of footprints cleaned by the player.</summary>
    public int footprintsCleanedCount = 0;

    [Header("Cutscene & UI Fade")]
    ///<summary>Camera GameObject for cutscenes.</summary>
    public GameObject cameraGameObject;

    ///<summary>Canvas containing the animator for cutscenes.</summary>
    public Canvas animatorCanvas;

    ///<summary>Fade duration for UI transitions.</summary>
    public float fadeDuration = 2f;

    ///<summary>UI for showing mop availability.</summary>
    public GameObject mopUI;

    // ------------------ Update & Interaction Handling ------------------ //

    ///<summary>
    /// Detects interactable objects in front of the player each frame,  
    /// displays prompts, and prepares references for interaction logic.
    ///</summary>
    void Update()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Coroutine handling store clerk cutscene and scene transition after stealing an item.
    ///</summary>
    private IEnumerator StoreClerkSequence()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Teleports the player to the assigned spawn point.
    ///</summary>
    public void Respawn()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Handles single-press interaction logic for all interactable objects.
    ///</summary>
    void OnInteract()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Toggles mop equip state and visibility.
    ///</summary>
    void OnEquip()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Starts the dialogue sequence with the friend NPC.
    ///</summary>
    void StartFriendConversation()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Ends the dialogue sequence with the friend NPC.
    ///</summary>
    void EndFriendConversation()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Handles hold-to-interact logic for shelves, candy piles, and footprints.
    ///</summary>
    void HandleHoldInteraction()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>Resets the lifted shelves counter and updates UI.</summary>
    public void ResetShelvesLiftedCount() { /* ... */ }

    ///<summary>Decreases the lifted shelves counter and updates UI.</summary>
    public void DecreaseShelvesLiftedCount() { /* ... */ }

    ///<summary>Decreases the cleaned footprints counter and updates UI.</summary>
    public void DecreaseFootprintsCleanedCount() { /* ... */ }

    ///<summary>Updates UI for cleaned footprints and checks task completion.</summary>
    public void UpdateFootprintsUI() { /* ... */ }

    ///<summary>Updates UI for lifted shelves and checks task completion.</summary>
    public void UpdateShelvesUI() { /* ... */ }

    ///<summary>
    /// Checks if all main objectives are completed  
    /// and triggers the end-game sequence if so.
    ///</summary>
    void CheckAllTasksCompleted()
    {
        // [Logic unchanged from original code...]
    }

    ///<summary>
    /// Coroutine for the game completion cutscene sequence before returning to main menu.
    ///</summary>
    private IEnumerator CompleteGameSequence()
    {
        // [Logic unchanged from original code...]
    }
}
