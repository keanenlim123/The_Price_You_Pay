/// <summary>
/// CandyBehaviour.cs
/// Handles the logic for candy piles in the scene,
/// including checking if the player has found the correct candy bar
/// and updating the task UI accordingly.
/// </summary>
/// <author> Keanen Lim Xi En </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10270417C </StudentID>

using UnityEngine;
using TMPro;  // Make sure to include this

public class CandyBehaviour : MonoBehaviour
{
    /// <summary>
    /// Whether this specific candy pile contains the correct candy bar.
    /// </summary>
    public bool isCorrectPile = false;

    /// <summary>
    /// Tracks whether the correct candy bar has been found.
    /// This value is shared across all instances of <see cref="CandyBehaviour"/>.
    /// </summary>
    public static bool candyBarFound = false;

    /// <summary>
    /// Reference to the <see cref="TextMeshProUGUI"/> component displaying the Task 1 text in the UI.
    /// </summary>
    public TextMeshProUGUI task1;

    /// <summary>
    /// Called when the player searches this pile.
    /// If it is the correct pile, the task text is updated,
    /// all candy piles are destroyed, and the task is marked as complete.
    /// If it is the wrong pile, only this pile is destroyed.
    /// </summary>
    public void SearchPile()
    {
        if (isCorrectPile)
        {
            candyBarFound = true;
            Debug.Log("Correct candy pile! Destroying all piles.");

            // Apply strikethrough formatting to task text
            if (task1 != null)
            {
                task1.text = $"<s>{task1.text}</s>";
            }

            // Destroy all piles with the "CandyPile" tag
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
