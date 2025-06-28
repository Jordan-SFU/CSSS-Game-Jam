using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    // Function to load Level1
    public void LoadLevel1()
    {
        Debug.Log("Loading Level1...");
        SceneManager.LoadScene("Level1");
    }

    // Function to exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
