using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator cutsceneAnimator;  // Animator for cutscene
    public GameObject audioObject;     // Audio GameObject to unhide
    public GameObject buildObject;  
    

    public void PlayGame()
    {
        Debug.Log("Game started!");
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // Play cutscene
        cutsceneAnimator.SetTrigger("PlayCutscene");

        // Enable audio
        audioObject.SetActive(true);

        buildObject.SetActive(true);

        // Wait for 33 seconds
        yield return new WaitForSeconds(40f);

        // Load the next scene in build order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Debug.Log("Next scene loaded after 40 seconds.");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
