using UnityEngine;

public class CameraSound : MonoBehaviour
{
    private AudioSource cameraAudioSource;
    public AudioClip environmentSound;

    private void Awake()
    {
        cameraAudioSource = GetComponent<AudioSource>();
        cameraAudioSource.clip = environmentSound;
    }

    private void Start()
    {
        cameraAudioSource.Play();
    }

    private void OnDisable()
    {
        cameraAudioSource.Stop();
    }
}
