
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TreeChopper : MonoBehaviour
{
    public event Action onTreeDamage;
    [SerializeField] private GameObject currentTree;
    [SerializeField] private AudioClip chopSound;

    private ThirdPersonController controller;
    private Animator playerAnimator;
    private int clipCount;

    private void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            playerAnimator.SetBool("isChopping", false);
            controller.isInteraction = false;
            Cursor.visible = false;
        }
        if (currentTree != null && currentTree.GetComponent<TreeInteraction>().treeChopped)
        {
            StopChopping();
        }
    }

    public void StartChopping(Collider collider)
    {
        currentTree = collider.gameObject;
        controller.isInteraction = true;

        Vector3 treePos = currentTree.transform.position;
        treePos.y = transform.position.y;
        transform.LookAt(treePos);
        playerAnimator.SetBool("isChopping", true);
    }

    private void StopChopping()
    {
        playerAnimator.SetBool("isChopping", false);
        controller.isInteraction = false;
        currentTree.GetComponent<TreeInteraction>().Chopped();
    }

    private void OnChop(AnimationEvent animationEvent)
    {
        onTreeDamage.Invoke();
    }

    private void OnChopSound(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(chopSound, currentTree.transform.position);
        currentTree.GetComponent<Animation>().Play();
    }
}
