using System;
using System.Collections.Generic;
using UnityEngine;

/*Class for definition of saving datas*/

[Serializable]
public class GameState
{
    //Player
    public float[] _playerPosition;
    public float[] _playerRotation;

    //PlayerInventory
    public List<bool> inventoryItemsExist = new();
    public List<string> inventoryItemsNames = new ();
    public List<int> inventoryItemsStacks = new ();

    //Scene
    public string _sceneName;

    public GameState()
    {
        _playerPosition = new float[3];
        _playerRotation = new float[3];
    }
}
