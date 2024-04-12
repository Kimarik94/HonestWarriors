using System;
using UnityEngine;

public class WolfAnimEvents : MonoBehaviour
{
    public bool attackTouch = false;

    private Animation _wolfAnimation;

    void Start()
    {
        _wolfAnimation = GetComponent<Animation>();
    }

    private void OnAttackTouch(AnimationEvent animEvent)
    {
        attackTouch = true;
    }
}
