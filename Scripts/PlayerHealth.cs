using System;
using UnityEngine;

public class PlayerHealth : Character
{
    [Header("Player Settings")]
    public int playerID;
    public HealthBar healthBar;

    private UIManager ui;

    [Header("Runtime Health")]
    [SerializeField] private int startingHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (gameObject.tag == "player1")
            healthBar = UnityEngine.GameObject.Find("Player1HealthBar").GetComponent<HealthBar>();
        if (gameObject.tag == "player2")
            healthBar = UnityEngine.GameObject.Find("Player2HealthBar").GetComponent<HealthBar>();
    }

    private void Start()
    {
        ui = FindFirstObjectByType<UIManager>();

        if (startingHealth <= 0)
            startingHealth = (int)startingHitPoints;

        CurrentHealth = startingHealth;
        IsDead = false;

        UpdateUI();
    }

    public bool AdjustHitPoints(int amount, int attackerID = 0)
    {
        if (IsDead) return false;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)maxHitPoints);

        // Check death
        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }

        UpdateUI();
        return true;
    }

    public void ResetHealth()
    {
        CurrentHealth = (int)startingHitPoints;
        IsDead = false;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ui != null)
        {
            ui.UpdateHealth(playerID, CurrentHealth, (int)maxHitPoints);
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHealth, (int)maxHitPoints);
        }
    }

    public void OnDamaged()
    {
        AdjustHitPoints(-10);
    }
}
