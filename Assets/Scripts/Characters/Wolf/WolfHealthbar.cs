using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WolfHealthbar : MonoBehaviour
{
    [SerializeField] private WolfCharacteristics wolfCharacteristics;
    [SerializeField] private Image foregroundImage;
    private float updateSpeedSeconds = 0.5f;

    private void Start()
    {
        foregroundImage = GameObject.Find("WolfHBForeground").GetComponent<Image>();
        wolfCharacteristics = GetComponentInParent<WolfCharacteristics>();
        wolfCharacteristics.onHealthDecrease += HandleHealthDecrease;
        wolfCharacteristics.onHealthIncrease += HandleHealthIncrease;
    }

    private void Update()
    {
        transform.eulerAngles = Camera.main.transform.eulerAngles;
        if(wolfCharacteristics.isDie) gameObject.SetActive(false);
    }

    private void HandleHealthDecrease(float percent)
    {
        StartCoroutine(ChangeToPercent(percent));
    }
    private void HandleHealthIncrease(float hpPerSecond)
    {
        StartCoroutine(RechargeHp(hpPerSecond));
    }
    private IEnumerator RechargeHp(float maxHealth)
    {
        float preChangedHp = foregroundImage.fillAmount;
        float elapsed = 0f;
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangedHp, maxHealth, elapsed / updateSpeedSeconds);
            yield return null;
        }
        foregroundImage.fillAmount = maxHealth;
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
