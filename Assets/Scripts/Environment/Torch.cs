using UnityEngine;

public class Torch : MonoBehaviour
{
    public AudioClip _torchSound;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(_torchSound,transform.position);
    }
}
