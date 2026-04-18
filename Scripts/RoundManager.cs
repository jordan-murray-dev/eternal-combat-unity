using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public float roundTime = 60f; // seconds
    private float timer;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("End of Round UI")]
    public GameObject endRoundCanvas;
    public Button mainMenuButton;
    public Button continueButton;
    public TextMeshProUGUI roundResultText;

    [Header("Players")]
    public GameObject player1;
    public GameObject player2;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    private bool roundOver = false;
    public bool IsRoundOver => roundOver;

    private void Start()
    {
        timer = roundTime;

        if (endRoundCanvas != null)
            endRoundCanvas.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinuePressed);

        UpdateTimerUI();
    }

    public void Initialize(GameObject player1, GameObject player2)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.player1Health = this.player1.GetComponent<PlayerHealth>();
        this.player2Health = this.player2.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (roundOver) return;

        timer -= Time.deltaTime;
        UpdateTimerUI();

        // Check for round end
        if (timer <= 0f)
        {
            EndRound();
            return;
        }

        if (player1Health != null && player2Health != null)
        {
            if (player1Health.CurrentHealth <= 0 || player2Health.CurrentHealth <= 0)
            {
                EndRound();
                return;
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void EndRound()
    {
        string resultText = "";

        roundOver = true;

        if(endRoundCanvas != null)
        {
            endRoundCanvas.SetActive(true);
        }

        // Round ended because of time
        if (timer <= 0f)
        {
            resultText = "Time's up!";

            // Determine who has more health
            if (player1Health != null && player2Health != null)
            {
                if (player1Health.CurrentHealth > player2Health.CurrentHealth)
                {
                    resultText = $"{PlayerPrefs.GetString("Player1Name", "Player1")} wins!";
                    ScoreManager.Instance.AddWinPoints(1);
                }
                else if (player2Health.CurrentHealth > player1Health.CurrentHealth)
                {
                    resultText = $"{PlayerPrefs.GetString("Player2Name", "Player2")} wins!";
                    ScoreManager.Instance.AddWinPoints(2);
                }
            }
        }
        // Round ended because a player died
        else if (player1Health.CurrentHealth <= 0 && player2Health.CurrentHealth > 0)
        {
            resultText = $"{PlayerPrefs.GetString("Player2Name", "Player2")} wins!";
            ScoreManager.Instance.AddWinPoints(2);
        }
        else if (player2Health.CurrentHealth <= 0 && player1Health.CurrentHealth > 0)
        {
            resultText = $"{PlayerPrefs.GetString("Player1Name", "Player1")} wins!";
            ScoreManager.Instance.AddWinPoints(1);
        }

        if (roundResultText != null)
            roundResultText.text = resultText;
    }


    public void OnMainMenuPressed()
    {
        LeaderboardManager.Instance.SubmitFinalScores();

        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator DelayInitialize()
    {
        yield return null;
        LeaderboardManager.Instance.ReInitialize();
    }

    public void OnContinuePressed()
    {
        if (endRoundCanvas != null)
            endRoundCanvas.SetActive(false);

        // Respawn players
        if (player1 != null && player1Spawn != null)
            player1.transform.position = player1Spawn.position;
        if (player2 != null && player2Spawn != null)
            player2.transform.position = player2Spawn.position;

        // Reset health
        player1Health?.ResetHealth();
        player2Health?.ResetHealth();

        // Reset timer
        timer = roundTime;
        roundOver = false;
    }

    public void OnPlayerHit(int playerNumber)
    {
        ScoreManager.Instance.AddHitPoints(playerNumber);
    }
}
