using UnityEngine;

public class InteractionUIButton : MonoBehaviour
{
    private Animator _animator;
    private ThirdPersonController _playerController;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GameObject.Find("Player").GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        if (_playerController._focus != null) _animator.SetBool("Focused", true);
        else _animator.SetBool("Focused", false);
    }
}
