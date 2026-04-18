using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Image meterImage;       
    public TextMeshProUGUI hpText;

    public void SetHealth(int currentHP, int maxHP)
    {
        if (meterImage != null)
        {
            meterImage.fillAmount = Mathf.Clamp01((float)currentHP / maxHP);
        }

        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP}";
        }
    }
}
