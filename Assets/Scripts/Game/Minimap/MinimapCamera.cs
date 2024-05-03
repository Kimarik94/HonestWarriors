using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Camera _camera;
    public GameObject _player;

    public void Start()
    {
        _player = GameObject.Find("Player(Clone)");
    }

    public void Update()
    {
        _camera.transform.position = new Vector3(_player.transform.position.x, 35, _player.transform.position.z);
    }
}
