using System;
using UnityEngine;
using UnityEngine.UI;

public class AbillityItem : MonoBehaviour
{
    public event Action<string> OnUseSkill;

    public Image _abillityImage;
    public Image _abillityCooldownImage;
    public GameObject _abillityNumber;
    public float _cooldownTime = 5f;
    public bool _isCooldown = false;
    public KeyCode _abillityKey;
    public AudioClip _abillitySound;

    private PlayerBattleActions _battleActions;

    public void Start()
    {
        _battleActions = GameObject.Find("Player(Clone)").GetComponent<PlayerBattleActions>();
        if (_abillityCooldownImage != null)
        _abillityCooldownImage.fillAmount = 0;
    }

    public void Update()
    {
        if (_abillityCooldownImage != null) AbillityAction();
    }

    void AbillityAction()
    {
        if (Input.GetKey(_abillityKey) && !_isCooldown && !_battleActions._skillActive)
        {
            if (_abillitySound != null)
            {
                AudioSource.PlayClipAtPoint(_abillitySound, GameObject.Find("Player(Clone)").transform.position);
            }

            _isCooldown = true;
            _abillityCooldownImage.fillAmount = 1;
            OnUseSkill?.Invoke(_abillityKey.ToString());
        }

        if (_isCooldown)
        {
            _abillityCooldownImage.fillAmount -= 1 / _cooldownTime * Time.deltaTime;

            if(_abillityCooldownImage.fillAmount <= 0)
            {
                _abillityCooldownImage.fillAmount = 0;
                _isCooldown = false;
            }
        }

    }
}
