using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamebahaivour : MonoBehaviour
{
    public GameObject _pauseMenu;
    public GameObject _deadMenu;
    public InventoryUI _inventoryUI;

    public static bool _isPaused;

    private PlayerCharacteristics _playerCharacteristics;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _isPaused = false;
        _pauseMenu.SetActive(false);
        _deadMenu.SetActive(false);
        _inventoryUI = GameObject.Find("GUI").GetComponent<InventoryUI>();
        _playerCharacteristics = GameObject.Find("Player").GetComponent<PlayerCharacteristics>();
    }

    void Update()
    {
        if (_inventoryUI._isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!_playerCharacteristics._isDie)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _isPaused = !_isPaused;
                if(_isPaused) PauseGame();
                if(!_isPaused) ResumeGame();
            }
        }
        if (_playerCharacteristics._isDie && !_isPaused)
        {
            _isPaused = true;
            _deadMenu.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        _isPaused = false;
        Destroy(GameObject.Find("Player"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        _pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
