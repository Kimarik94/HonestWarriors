using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamebahaivour : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject deadMenu;
    public InventoryUI inventoryUI;

    public static bool isPaused;

    private PlayerCharacteristics playerCharacteristics;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        isPaused = false;
        pauseMenu.SetActive(false);
        deadMenu.SetActive(false);
        inventoryUI = GameObject.Find("GUI").GetComponent<InventoryUI>();
        playerCharacteristics = GameObject.Find("Player").GetComponent<PlayerCharacteristics>();
    }

    void Update()
    {
        if (inventoryUI.isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!playerCharacteristics.isDie)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                isPaused = !isPaused;
                if(isPaused) PauseGame();
                if(!isPaused) ResumeGame();
            }
        }
        if (playerCharacteristics.isDie && !isPaused)
        {
            isPaused = true;
            deadMenu.SetActive(true);
        }
    }

    public void RestartGame()
    {
        isPaused = false;
        Destroy(GameObject.Find("Player"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
