using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WolfHealthbar : MonoBehaviour
{
    [SerializeField] private WolfCharacteristics _wolfCharacteristics;
    [SerializeField] private Image _foregroundImage;
    private float updateSpeedSeconds = 0.5f;

    private void Start()
    {
        _foregroundImage = GameObject.Find("WolfHBForeground").GetComponent<Image>();
        _wolfCharacteristics = GetComponentInParent<WolfCharacteristics>();
        _wolfCharacteristics.onHealthDecrease += HandleHealthDecrease;
        _wolfCharacteristics.onHealthIncrease += HandleHealthIncrease;
    }

    private void Update()
    {
        transform.eulerAngles = Camera.main.transform.eulerAngles;
        if(_wolfCharacteristics._isDie) gameObject.SetActive(false);
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
        float preChangedHp = _foregroundImage.fillAmount;
        float elapsed = 0f;
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            _foregroundImage.fillAmount = Mathf.Lerp(preChangedHp, maxHealth, elapsed / updateSpeedSeconds);
            yield return null;
        }
        _foregroundImage.fillAmount = maxHealth;
    }
    private IEnumerator ChangeToPercent(float percent)
    {
        float preChangePercent = _foregroundImage.fillAmount;
        float targetPercent = percent;
        float elapsed = 0f;
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(preChangePercent, targetPercent, elapsed / updateSpeedSeconds);
            _foregroundImage.fillAmount = newFillAmount;
            yield return null;
        }
        _foregroundImage.fillAmount = targetPercent;
    }
}
