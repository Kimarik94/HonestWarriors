using UnityEngine;
using UnityEngine.UI;

public class AbillitySystem : MonoBehaviour
{
    public AbillityItem[] _abillities;

    private void Start()
    {
        for (int i = 0; i < _abillities.Length; i++)
        {
            if (_abillities[i] != null)
            {
                _abillities[i]._abillityNumber.GetComponent<Text>().text = (i + 1).ToString();
            }
        }
    }
}
