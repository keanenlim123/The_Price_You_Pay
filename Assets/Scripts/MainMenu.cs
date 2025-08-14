/// <summary>
/// MainMenu.cs
/// This script manages the main menu functionality, including starting the game with a cutscene,
/// enabling necessary objects (audio & build), and transitioning to the next scene.
/// It also allows the player to quit the game.
/// </summary>
/// <author> Lim Xue Zhi Conan </author>
/// <date> 7/8/2025 </date>
/// <Student ID> S10269214H </Student ID>
 
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    ///<summary>
    /// Animator component responsible for playing the cutscene.
    ///</summary>
    public Animator cutsceneAnimator;

    ///<summary>
    /// Audio GameObject that will be activated when the cutscene starts.
    ///</summary>
    public GameObject audioObject;

    ///<summary>
    /// Build GameObject to activate when the game starts.
    ///</summary>
    public GameObject buildObject;

    ///<summary>
    /// Called when the Play button is pressed.
    /// Starts the cutscene and prepares game objects before loading the next scene.
    ///</summary>
    public void PlayGame()
    {
        Debug.Log("Game started!");
        StartCoroutine(CutsceneSequence());
    }

    ///<summary>
    /// Coroutine that plays the cutscene, enables necessary objects,
    /// waits for a set duration, then loads the next scene.
    ///</summary>
    private IEnumerator CutsceneSequence()
    {
        // Play cutscene
        cutsceneAnimator.SetTrigger("PlayCutscene");

        // Enable audio
        audioObject.SetActive(true);

        // Enable build object
        buildObject.SetActive(true);

        // Wait for 40 seconds before changing scenes
        yield return new WaitForSeconds(40f);

        // Load the next scene in build order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Debug.Log("Next scene loaded after 40 seconds.");
    }

    ///<summary>
    /// Quits the application when the Quit button is pressed.
    ///</summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
