using UnityEngine;

public class FallingTreeBehaivour : MonoBehaviour
{
    [SerializeField] private AudioClip _treeFallingSound;
    [SerializeField] private GameObject _treeStump;
    [SerializeField] private Collider _fallingTreeCollider;
    private float _startLifeTime;

    private void OnEnable()
    {
        _fallingTreeCollider = GetComponent<Collider>();
        AudioSource.PlayClipAtPoint(_treeFallingSound, transform.position);
        Vector3 instantiationPos = transform.position;
        instantiationPos.y = GameObject.Find("CinemachineFollowTarget").transform.position.y + 0.1f;
        instantiationPos.x += -0.5f;
        instantiationPos.z += -0.5f;
        Instantiate(_treeStump, instantiationPos, Quaternion.identity);
        _startLifeTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _startLifeTime > 10f)
        {
            _fallingTreeCollider.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
