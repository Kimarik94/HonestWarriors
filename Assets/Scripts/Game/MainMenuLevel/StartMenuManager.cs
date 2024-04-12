using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("FirstLevel");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
