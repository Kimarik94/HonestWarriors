using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    public List<SaveSlot> _slots;
    private string folderPath;

    private void Start()
    {
        folderPath = Path.Combine(Application.persistentDataPath, SaveLoadSystem.folderName);

        foreach (SaveSlot slot in GetComponentsInChildren<SaveSlot>())
        {
            _slots.Add(slot);
        }
    }

    public void RefreshSaveSlots()
    {
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            for(int i = 0; i < files.Length; i++)
            {
                string pathForLoadLink = files[i];
                string temp = files[i];
                temp = temp.Substring(temp.LastIndexOf("\\") + 1);
                _slots[i]._tmPro.text = temp.Substring(0,temp.LastIndexOf("."));

                if (_slots[i]!= null && _slots[i]._tmPro.text.Contains("save"))
                {
                    _slots[i].loadButton.image.color = new Color(255f, 255f, 255f, 255f);
                    _slots[i].loadButton.interactable = true;
                    _slots[i].deleteButton.image.color = new Color(255f, 255f, 255f, 255f);
                    _slots[i].deleteButton.interactable = true;
                    _slots[i].loadLink = files[i];
                    _slots[i].loadButton.onClick.AddListener(_slots[i].LoadData);
                }
            }
        }
        else
        {
            Console.WriteLine("Directory not found: " + folderPath);
        }
    }
}
