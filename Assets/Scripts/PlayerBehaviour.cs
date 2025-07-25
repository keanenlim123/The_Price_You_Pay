using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    bool canInteract = false;
    DoorBehaviour currentDoor = null;

    [SerializeField]
    float interactRange = 2f;
    [SerializeField]
    float rayHeightOffset = 1.0f;

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

            }
            Debug.DrawRay(rayOrigin, transform.forward * interactRange, Color.green);
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
        }
    }
}