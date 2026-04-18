using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header ("Player UI")]
    public TextMeshProUGUI player1Display;
    public TextMeshProUGUI player2Display;

    [Header("Health Bars")]
    public HealthBar player1HealthBar;
    public HealthBar player2HealthBar;

    [Header("Health Text")]
    public TextMeshProUGUI player1HPText;
    public TextMeshProUGUI player2HPText;

    [Header("Timer")]
    public TextMeshProUGUI timer;

    private string name1;
    private string name2;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        name1 = PlayerPrefs.GetString("Player1Name", "Player 1");
        name2 = PlayerPrefs.GetString("Player2Name", "Player 2");

        UpdateScore(1, 0);
        UpdateScore(2, 0);
    }

    public void UpdateScore(int playerNumber, int score)
    {
        if (playerNumber == 1)
        {
            player1Display.text = $"{name1} - {score}";
        }
        else if (playerNumber == 2)
        {
            player2Display.text = $"{name2} - {score}";
        }
    }

    public void UpdateHealth(int playerNumber, int currentHP, int maxHP)
    {
        if (playerNumber == 1 && player1HealthBar != null)
            player1HealthBar.SetHealth(currentHP, maxHP);
        else if (playerNumber == 2 && player2HealthBar != null)
            player2HealthBar.SetHealth(currentHP, maxHP);
    }
}
