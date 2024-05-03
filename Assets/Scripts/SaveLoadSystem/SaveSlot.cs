using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private Inventory inventory;
    public TextMeshProUGUI _tmPro;
    public Button loadButton;
    public Button deleteButton;

    public string loadLink;

    private void Start()
    {
        inventory = GameObject.Find("GUI").GetComponent<Inventory>();

        loadButton.interactable = false;
        loadButton.image.color = new Color(255f, 255f, 255f, 0f);
        deleteButton.interactable = false;
        deleteButton.image.color = new Color(255f, 255f, 255f, 0f);
    }

    public void LoadData()
    {
        GameObject.Find("GameManager").GetComponent<SaveLoadManager>().LoadData(this);
    }

    public void DeleteData() 
    {
        //To do delete save slot
    }
}
