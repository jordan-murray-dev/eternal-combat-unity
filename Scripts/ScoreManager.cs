using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Scoring Settings")]
    public int hitPoints = 10;
    public int winPoints = 50;

    private int player1Score;
    public int Player1Score
    {
        get => player1Score;
        private set
        {
            player1Score = value;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateScore(1, player1Score);
        }
    }

    private int player2Score;
    public int Player2Score
    {
        get => player2Score;
        private set
        {
            player2Score = value;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateScore(2, player2Score);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize scores
        Player1Score = 0;
        Player2Score = 0;
    }

    public void AddHitPoints(int playerNumber)
    {
        if(playerNumber == 1)
            Player1Score += hitPoints;
        else if (playerNumber == 2)
        {
            Player2Score += hitPoints;
        }
    }

    public void AddWinPoints(int playerNumber)
    {
        if (playerNumber == 1)
            Player1Score += winPoints;
        else if (playerNumber == 2)
            Player2Score += winPoints;
    }

    public void ResetScores()
    {
        Player1Score = 0;
        Player2Score = 0;
    }

     public int GetHighestScore()
    {
        return Mathf.Max(Player1Score, Player2Score);
    }
}
