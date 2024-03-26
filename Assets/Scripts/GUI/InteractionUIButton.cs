using UnityEngine;

public class InteractionUIButton : MonoBehaviour
{
    private Animator _animator;
    private ThirdPersonController playerController;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        playerController = GameObject.Find("Player").GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        if (playerController.focus != null) _animator.SetBool("Focused", true);
        else _animator.SetBool("Focused", false);
    }
}
