using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private PlayerCharacteristics playerCharacteristics;
    private Image foregroundImage;
    private float updateSpeedSeconds = 0.5f;

    void Start()
    {
        foregroundImage = GameObject.FindWithTag("HealthbarImage").GetComponent<Image>();
        playerCharacteristics = GameObject.Find("Player").GetComponent<PlayerCharacteristics>();
        playerCharacteristics.onHealthDecrease += HandleHealthDecrease;
        playerCharacteristics.onHealthIncrease += HandleHealthIncrease;
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
        float preChangedHp = foregroundImage.fillAmount;
        float elapsed = 0f;
        while(elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangedHp, passiveHealPerSecond, elapsed / updateSpeedSeconds);
            yield return null;
        }
        foregroundImage.fillAmount = passiveHealPerSecond;
    }

    private IEnumerator ChangeToPercent(float percent)
    {
        float preChangePercent = foregroundImage.fillAmount;
        float targetPercent = percent;
        float elapsed = 0f;
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(preChangePercent, targetPercent, elapsed / updateSpeedSeconds);
            foregroundImage.fillAmount = newFillAmount;
            yield return null;
        }
        foregroundImage.fillAmount = targetPercent;
    }
}
