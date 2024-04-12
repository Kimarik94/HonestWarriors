using UnityEngine;

public class PlayerWeaponsEquip : MonoBehaviour
{
    private GameObject _axeUnEquiped;
    private GameObject _axeEquiped;
    private GameObject _axeDamageCollider;

    public bool _isWeaponEquiped = true;
    [SerializeField] private bool _equip = false;
    [SerializeField] private bool _unEquip = true;

    private Animator _playerAnimator;
    private PlayerInputHandler _playerInputHandler;
    private ThirdPersonController _playerController;
    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerController = GetComponent<ThirdPersonController>();
        _axeEquiped = GameObject.Find("BattleAxe_GEO_Equiped");
        _axeUnEquiped = GameObject.Find("AxeUnEquiped");
        _axeDamageCollider = GameObject.Find("Axe");
        _axeUnEquiped.SetActive(false);
    }

    private void Update()
    {
        if (_playerInputHandler._WeaponEquipPressed && !_playerController._isAttacking)
        {
            if (_isWeaponEquiped)
            {
                _playerAnimator.SetBool("UnEquip", _unEquip);
                _unEquip = false;
                _equip = true;
            }
            else if (!_isWeaponEquiped)
            {
                _playerAnimator.SetBool("Equip", _equip);
                _unEquip = true;
                _equip = false;
            }
        }
    }

    private void OnEquip(AnimationEvent animationEvent)
    {
        _axeUnEquiped.SetActive(_isWeaponEquiped);
        _axeEquiped.SetActive(!_isWeaponEquiped);
        _axeDamageCollider.SetActive(!_isWeaponEquiped);
    }

    private void OnEquipEnd(AnimationEvent animationEvent)
    {
        _isWeaponEquiped = !_isWeaponEquiped;
    }
}
