using UnityEngine;

public class CameraSound : MonoBehaviour
{
    private AudioSource _cameraAudioSource;
    public AudioClip _environmentSound;

    private void Awake()
    {
        _cameraAudioSource = GetComponent<AudioSource>();
        _cameraAudioSource.clip = _environmentSound;
    }

    private void Start()
    {
        _cameraAudioSource.Play();
    }

    private void OnDisable()
    {
        _cameraAudioSource.Stop();
    }
}
