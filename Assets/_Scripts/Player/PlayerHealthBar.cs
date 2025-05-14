using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            int maxHP = GameManager.Instance.PlayerHealth;
            slider.minValue = 0;
            slider.maxValue = maxHP;
            slider.value = maxHP;

            GameManager.Instance.OnLifeValueChanged.AddListener(UpdateHealth);
        }
    }

    private void UpdateHealth(int currentHealth)
    {
        slider.value = currentHealth;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLifeValueChanged.RemoveListener(UpdateHealth);
    }
}
