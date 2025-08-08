using UnityEngine;

public class CandyBehaviour : MonoBehaviour
{
    public bool isCorrectPile = false;

    public void SearchPile()
    {
        if (isCorrectPile)
        {
            Debug.Log("Correct candy pile! Destroying all piles.");
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
