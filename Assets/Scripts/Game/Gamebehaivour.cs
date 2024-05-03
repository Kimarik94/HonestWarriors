using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamebahaivour : MonoBehaviour
{
    public GameObject _pausePanel;
    public GameObject _deathPanel;
    public GameObject _minimap;

    public Button pauseMenuContinueButton;
    public Button deathPanelExitGame;

    public static bool _isPaused;

    private PlayerCharacteristics _playerCharacteristics;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        _pausePanel = GameObject.Find("PausePanel");

        _deathPanel = GameObject.Find("DeathPanel");
        deathPanelExitGame = GameObject.Find("DeathPanelExitButton").GetComponent<Button>();

        if (deathPanelExitGame.onClick.GetPersistentEventCount() == 0) deathPanelExitGame.onClick.AddListener(ExitGame);

        _minimap = GameObject.Find("Minimap");
        pauseMenuContinueButton = GameObject.Find("PauseMenuContinueButton").GetComponent<Button>();
        pauseMenuContinueButton.onClick.AddListener(ResumeGame);

        _playerCharacteristics = GameObject.Find("Player(Clone)").GetComponent<PlayerCharacteristics>();

        _isPaused = false;

        _pausePanel.SetActive(false);
        _deathPanel.SetActive(false);
    }

    void Update()
    {
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
            _deathPanel.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        _isPaused = false;
        Destroy(GameObject.Find("Player(Clone)"));
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
        _pausePanel.SetActive(true);
        _minimap.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _deathPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _minimap.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
