using System;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private Animation _playerAnimation;
    private AbillitySystem _abillitySystem;

    public event Action onTreeChop;
    public event Action<string> onEnemyHit;

    private string _skillType;

    void Start()
    {
        _playerAnimation = GetComponent<Animation>();
        _abillitySystem = GameObject.Find("BorderForSkills").GetComponent<AbillitySystem>();

        foreach (var _abillity in _abillitySystem._abillities)
        {
            if (_abillity != null) _abillity.OnUseSkill += MonitorSkillNumber;
        }
    }

    private void MonitorSkillNumber(string number)
    {
        _skillType = number;
    }

    private void OnDestroy()
    {
        if(_abillitySystem != null)
        {
            foreach (var _abillity in _abillitySystem._abillities)
            {
                if (_abillity != null) _abillity.OnUseSkill -= MonitorSkillNumber;
            }
        }
    }

    private void OnStartTreeChop(AnimationEvent animationEvent)
    {
        onTreeChop?.Invoke();
    }

    private void OnEnemyHit(AnimationEvent animationEvent)
    {
        onEnemyHit?.Invoke(_skillType);
    }
}
