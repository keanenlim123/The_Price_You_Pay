using UnityEngine;
using TMPro;  // Make sure to include this

public class CandyBehaviour : MonoBehaviour
{
    public bool isCorrectPile = false;

    public static bool candyBarFound = false;

    // Reference to the TextMeshProUGUI component for task1 text
    public TextMeshProUGUI task1;

    public void SearchPile()
    {
        if (isCorrectPile)
        {
            candyBarFound = true;
            Debug.Log("Correct candy pile! Destroying all piles.");

            // Add strikethrough by wrapping text with <s> tags
            if (task1 != null)
            {
                task1.text = $"<s>{task1.text}</s>";
            }

            GameObject[] allPiles = GameObject.FindGameObjectsWithTag("CandyPile");
            foreach (GameObject pile in allPiles)
            {
                Destroy(pile);
            }
        }
        else
        {
            Debug.Log("Wrong pile. Keep searching!");
            Destroy(gameObject);
        }
    }
}
