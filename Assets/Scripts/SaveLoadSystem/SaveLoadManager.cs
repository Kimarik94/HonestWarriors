using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerPrefab;
    public GameObject playerSpawnPoint;
    public GameObject loadMenuPanel;
    public Gamebahaivour gamebahaivour;
    private GameState loadedState;
    public LoadMenu loadMenu;
    public Inventory inventory;
    private string fileName;
    private static int countSaves = 12;

    [Header("Buttons for Save/Load")]
    public Button saveButton;
    public Button loadButton;
    public Button closeSaveLoadPanelButton;

    Vector3 tempPos;
    Vector3 tempRot;

    void Start()
    {
        playerSpawnPoint = GameObject.Find("PlayerSpawnPoint");
        player = GameObject.Find("Player(Clone)");
        gamebahaivour = GetComponentInParent<Gamebahaivour>();
        saveButton.onClick.AddListener(SaveGameData);
        loadButton.onClick.AddListener(OpenLoadPanel);
        closeSaveLoadPanelButton.onClick.AddListener(CloseLoadPanel);

        loadMenu = GameObject.Find("LoadMenu").GetComponentInChildren<LoadMenu>();

        loadMenuPanel = GameObject.Find("LoadMenuPanel");
        loadMenuPanel.SetActive(false);

        inventory = GameObject.Find("GUI").GetComponent<Inventory>();

        fileName = "save_data_" + DateTime.Now.Date + ".json";
    }

    private void Update()
    {
        if (player == null)
        {
            //Instantiate(playerPrefab, tempPos, Quaternion.Euler(tempRot));
            ReassignVariablesOnReload();
            InventoryUpdateAfterLoad();

            if (saveButton.onClick.GetPersistentEventCount() == 0) saveButton.onClick.AddListener(SaveGameData);
            if (loadButton.onClick.GetPersistentEventCount() == 0) loadButton.onClick.AddListener(OpenLoadPanel);
            if (closeSaveLoadPanelButton.onClick.GetPersistentEventCount() == 0) closeSaveLoadPanelButton.onClick.AddListener(CloseLoadPanel);
            if (gamebahaivour.pauseMenuContinueButton.onClick.GetPersistentEventCount() == 0)
                gamebahaivour.pauseMenuContinueButton.onClick.AddListener(gamebahaivour.ResumeGame);

            Time.timeScale = 1.0f;
        }
    }

    void SaveGameData()
    {
        GameState saveState = new GameState();
        int filesCount = Directory.GetFiles(Application.persistentDataPath + "/" + SaveLoadSystem.folderName).Length;
        if (filesCount == 0 || filesCount == countSaves) filesCount = 1;
        else filesCount++;

        fileName = filesCount + "_save_data_" + DateTime.Now.Date.ToShortDateString() + ".json";

        saveState._playerPosition[0] = player.transform.position.x;
        saveState._playerPosition[1] = player.transform.position.y;
        saveState._playerPosition[2] = player.transform.position.z;

        saveState._playerRotation[0] = player.transform.rotation.x;
        saveState._playerRotation[1] = player.transform.rotation.y;
        saveState._playerRotation[2] = player.transform.rotation.z;

        saveState._sceneName = SceneManager.GetActiveScene().name;

        foreach (ItemSlotInfo slotInfo in inventory._items)
        {
            saveState.inventoryItemsExist.Add(slotInfo._stacks == 0 ? false : true);
            saveState.inventoryItemsNames.Add(slotInfo._name);
            saveState.inventoryItemsStacks.Add(slotInfo._stacks);
        }
        
        SaveLoadSystem.SaveGame(saveState, fileName);
        loadMenu.RefreshSaveSlots();
    }

    void OpenLoadPanel()
    {
        loadMenuPanel.SetActive(true);
        loadMenu.RefreshSaveSlots();
    }

    void CloseLoadPanel()
    {
        loadMenuPanel.SetActive(false);
    }

    public void LoadData(SaveSlot saveSlot)
    {
        if (player != null) Destroy(player);
        loadedState = SaveLoadSystem.LoadGame<GameState>(saveSlot.loadLink);

        SceneManager.LoadScene(loadedState._sceneName);

        tempPos = new Vector3(loadedState._playerPosition[0],
                                                loadedState._playerPosition[1],
                                                loadedState._playerPosition[2]);

        tempRot = new Vector3(loadedState._playerRotation[0],
                                   loadedState._playerRotation[1],
                                   loadedState._playerRotation[2]);

        playerSpawnPoint.transform.position = tempPos;
        playerSpawnPoint.transform.rotation = Quaternion.Euler(tempRot);
    }

    private void ReassignVariablesOnReload()
    {

        playerSpawnPoint.GetComponent<SpawnPlayerObject>().RespawnPlayer();
        player = GameObject.Find("Player(Clone)");

        gamebahaivour = GetComponentInParent<Gamebahaivour>();
        loadMenuPanel = GameObject.Find("LoadMenuPanel");
        inventory = GameObject.Find("GUI").GetComponent<Inventory>();
        loadMenu = GameObject.Find("LoadMenu").GetComponentInChildren<LoadMenu>();

        saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        loadButton = GameObject.Find("LoadButton").GetComponent<Button>();
        closeSaveLoadPanelButton = GameObject.Find("CloseButton").GetComponent<Button>();

        gamebahaivour._pausePanel = GameObject.Find("PausePanel");
        gamebahaivour._deathPanel = GameObject.Find("DeathPanel");
        gamebahaivour._minimap = GameObject.Find("Minimap");

        gamebahaivour.pauseMenuContinueButton = GameObject.Find("PauseMenuContinueButton").GetComponent<Button>();

        CloseLoadPanel();
        
        gamebahaivour.ResumeGame();
    }

    private void InventoryUpdateAfterLoad()
    {
        inventory._items = new List<ItemSlotInfo>();

        for (int i = 0; i < loadedState.inventoryItemsExist.Count; i++)
        {
            if (!loadedState.inventoryItemsExist[i])
            {
                ItemSlotInfo slot = new ItemSlotInfo(null, 0);
                inventory._items.Add(slot);
            }
            else
            {
                if (loadedState.inventoryItemsNames[i].Contains("Wood"))
                {
                    WoodItem woodItem = new WoodItem();
                    ItemSlotInfo slot = new ItemSlotInfo(woodItem, loadedState.inventoryItemsStacks[i]);
                    slot._name = loadedState.inventoryItemsNames[i];
                    inventory._items.Add(slot);
                }

                else if (loadedState.inventoryItemsNames[i] == "Stone")
                {
                    StoneItem stoneItem = new StoneItem();
                    ItemSlotInfo slot = new ItemSlotInfo(stoneItem, loadedState.inventoryItemsStacks[i]);
                    slot._name = loadedState.inventoryItemsNames[i];
                    inventory._items.Add(slot);
                }
            }
        }
        inventory.RefreshInventory();
    }
}
