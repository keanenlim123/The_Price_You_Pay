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


    void Update()
    {
        RaycastHit hit;
        canInteract = false;

        Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, interactRange))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Door"))
            {
                canInteract = true;
                currentDoor = hitObject.GetComponent<DoorBehaviour>();
                currentMop = null;
            }
            if (hitObject.CompareTag("BucketMop"))
            {
                canInteract = true;
                currentMop = hitObject.GetComponent<BucketMop>();
                currentDoor = null;
            }
            Debug.DrawRay(rayOrigin, transform.forward * interactRange, Color.green);
        }
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
}