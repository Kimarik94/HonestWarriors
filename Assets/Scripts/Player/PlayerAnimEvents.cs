using System;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private Animation _playerAnimation;

    public event Action onTreeChop;
    public event Action onEnemyHit;

    void Start()
    {
        _playerAnimation = GetComponent<Animation>();
    }

    private void OnStartTreeChop(AnimationEvent animationEvent)
    {
        onTreeChop?.Invoke();
    }

    private void OnEnemyHit(AnimationEvent animationEvent)
    {
        onEnemyHit?.Invoke();
    }
}
