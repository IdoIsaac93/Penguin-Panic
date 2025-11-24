using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToInstructions()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene(2);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(3);
    }

    public void ExitGame()
    {
        Application.Quit();

        //Stops play mode in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
