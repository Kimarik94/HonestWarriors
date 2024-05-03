using UnityEngine;

public class PlayerBattleActions : MonoBehaviour
{
    private Animator _playerAnimator;
    private Inventory _inventory;
    private PlayerCharacteristics _playerCharacteristics;
    private PlayerInputHandler _playerInputHandler;
    private ThirdPersonController _playerTPPController;
    private AbillitySystem _abillitySystem;
    public GameObject _battleCryAura;

    private float _battleCryOffTimer = 10f;
    private float _battleCryLastOnTimer;

    public bool _isAttacking = false;
    public bool _skillActive = false;

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerTPPController = GetComponent<ThirdPersonController>();
        _playerCharacteristics = GetComponent<PlayerCharacteristics>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _inventory = GameObject.Find("GUI").GetComponent<Inventory>();
        _abillitySystem = GameObject.Find("BorderForSkills").GetComponent<AbillitySystem>();
        _battleCryAura = GameObject.Find("BattleCry");
        _battleCryAura.SetActive(false);
        foreach (var _abillity in _abillitySystem._abillities)
        {
            if(_abillity != null) _abillity.OnUseSkill += CheckSkillNumber;
        }
    }

    private void OnDestroy()
    {
        if (_abillitySystem != null)
        {
            foreach (var _abillity in _abillitySystem._abillities)
            {
                if (_abillity != null) _abillity.OnUseSkill -= CheckSkillNumber;
            }
        }
    }


    void Update()
    {
        if (!Gamebahaivour._isPaused)
        {
            if (!_playerTPPController._isInteraction && !_playerCharacteristics._isDie 
                && !_inventory._isInventoryOpen)
            {
                if(!_skillActive) Block();
                if (_playerInputHandler._isBlocking) _isAttacking = false;

                if (!_playerInputHandler._isBlocking && !_skillActive)
                {
                    StandardAttack();
                }
            }

            if (_inventory._isInventoryOpen) _playerAnimator.SetFloat("Speed", 0);

            BattleCryOff();
        }
    }

    private void StandardAttack()
    {
        _playerAnimator.SetBool("Attack", _playerInputHandler._AttackPressed);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void CheckSkillNumber(string skillNumber)
    {
        if (skillNumber == "Alpha1" && !_skillActive) BackHandAttack();
        if (skillNumber == "Alpha2" && !_skillActive) KickAttack();
        if (skillNumber == "Alpha3" && !_skillActive) RoundAttack();
        if (skillNumber == "Alpha4" && !_skillActive) ComboAttack();
        if (skillNumber == "Alpha5" && !_skillActive) BattleCry();
    }

    private void BackHandAttack()
    {
        _playerAnimator.SetTrigger("BackHandAttack");
        _skillActive = true;
    }

    private void KickAttack()
    {
        _playerAnimator.SetTrigger("Kick");
        _skillActive = true;
    }

    private void RoundAttack()
    {
        _playerAnimator.SetBool("RoundAttack", true);
        _skillActive = true;
    }

    private void ComboAttack()
    {
        _playerAnimator.SetTrigger("Combo");
        _skillActive = true;
    }

    private void BattleCry()
    {
        _battleCryAura.SetActive(true);
        _battleCryLastOnTimer = Time.time;
        _playerCharacteristics._dealDamage = 30f;
        _playerAnimator.SetBool("BattleCry", true);
        _skillActive = true;
    }

    private void BattleCryOff()
    {
        if(Time.time - _battleCryLastOnTimer >= _battleCryOffTimer)
        {
            _battleCryAura.SetActive(false);
            _playerCharacteristics._dealDamage = 20f;
        }
    }

    private void Block()
    {
        _playerAnimator.SetBool("Block", _playerInputHandler._BlockPressed);
    }

    private void OnAttackStart(AnimationEvent animationEvent)
    {
        _isAttacking = true;
    }

    private void OnAttackEnd(AnimationEvent animationEvent)
    {
        _isAttacking = false;
    }

    private void OnSkillEnd(AnimationEvent animationEvent)
    {
        _skillActive = false;
    }
}
