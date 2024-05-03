using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerObject : MonoBehaviour
{
    public GameObject playerPrefab;

    void Awake()
    {
        if(GameObject.Find("Player(Clone)") == null)
        {
            Instantiate(playerPrefab, transform.position, transform.rotation);
        }
    }

    public void RespawnPlayer()
    {
        if (GameObject.Find("Player(Clone)") == null)
        {
            Instantiate(playerPrefab, transform.position, transform.rotation);
        }
    }
}
