using UnityEngine;

public class WallUI : MonoBehaviour
{
    public GameObject uiPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("Player"))
      {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
      }
    }
}

