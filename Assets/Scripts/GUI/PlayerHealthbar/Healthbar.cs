using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private PlayerCharacteristics _playerCharacteristics;
    private Image _foregroundImage;
    private float _updateSpeedSeconds = 0.5f;

    void Start()
    {
        _foregroundImage = GameObject.FindWithTag("HealthbarImage").GetComponent<Image>();
        _playerCharacteristics = GameObject.Find("Player").GetComponent<PlayerCharacteristics>();
        _playerCharacteristics.onHealthDecrease += HandleHealthDecrease;
        _playerCharacteristics.onHealthIncrease += HandleHealthIncrease;
    }

    private void HandleHealthDecrease(float percent)
    {
        StartCoroutine(ChangeToPercent(percent));
    }
    private void HandleHealthIncrease(float hpPerSecond)
    {
        StartCoroutine(Regeneration(hpPerSecond));
    }

    private IEnumerator Regeneration(float passiveHealPerSecond)
    {
        float preChangedHp = _foregroundImage.fillAmount;
        float elapsed = 0f;
        while(elapsed < _updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            _foregroundImage.fillAmount = Mathf.Lerp(preChangedHp, passiveHealPerSecond, elapsed / _updateSpeedSeconds);
            yield return null;
        }
        _foregroundImage.fillAmount = passiveHealPerSecond;
    }

    private IEnumerator ChangeToPercent(float percent)
    {
        float preChangePercent = _foregroundImage.fillAmount;
        float targetPercent = percent;
        float elapsed = 0f;
        while (elapsed < _updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(preChangePercent, targetPercent, elapsed / _updateSpeedSeconds);
            _foregroundImage.fillAmount = newFillAmount;
            yield return null;
        }
        _foregroundImage.fillAmount = targetPercent;
    }
}
