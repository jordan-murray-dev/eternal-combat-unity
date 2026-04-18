using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Leaderboard Settings")]
    public string filename = "leaderboard.txt";
    public int maxEntries = 5;

    [Header("UI")]
    public TextMeshProUGUI tmpText;

    private string filePath;

    // Internal leaderboard entry
    [System.Serializable]
    public class Entry
    {
        public string playerName;
        public int score;

        public override string ToString() => $"{playerName}|{score}";

        public static Entry FromString(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            string[] parts = line.Split('|');
            if (parts.Length != 2) return null;
            if (!int.TryParse(parts[1], out int parsedScore)) return null;
            return new Entry { playerName = parts[0], score = parsedScore };
        }
    }

    public List<Entry> leaderboard = new List<Entry>();

    private void Awake()
    {
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, filename);
        LoadLeaderboard();
    }

    public void UpdateUI()
    {
        if (tmpText == null)
        {
            Debug.LogWarning("LeaderboardManager has no TMP text assigned.");
            return;
        }

        string text = "Top Scores:\n";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            text += $"{i + 1}. {leaderboard[i].playerName} - {leaderboard[i].score}\n";
        }

        Debug.Log("UpdateUI text:\n" + text);
        tmpText.text = text;
    }

    public void SubmitFinalScores()
    {
        Debug.Log("SubmitFinalScore called");
        string name1 = PlayerPrefs.GetString("Player1Name", "Player1");
        string name2 = PlayerPrefs.GetString("Player2Name", "Player2");

        int score1 = ScoreManager.Instance.Player1Score;
        int score2 = ScoreManager.Instance.Player2Score;

        // Only add the higher of the two scores
        if (score1 >= score2)
            AddScore(name1, score1);
        else
            AddScore(name2, score2);

    }

    public void AddScore(string name, int score)
    {
        Debug.Log($"AddScore called: {name} - {score}");
        leaderboard.Add(new Entry { playerName = name, score = score });
        leaderboard = leaderboard.OrderByDescending(e => e.score).Take(maxEntries).ToList();
        SaveLeaderboard();
    }

    public void SaveLeaderboard()
    {
        Debug.Log("Loading Leaderboard file from: " + filePath);
        Debug.Log("Saving contents:\n" + string.Join("\n", leaderboard.Select(e => e.ToString())));

        File.WriteAllLines(filePath, leaderboard.Select(e => e.ToString()));
    }

    public void LoadLeaderboard()
    {
        leaderboard.Clear();

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "");
            return;
        }

        foreach (string line in File.ReadAllLines(filePath))
        {
            Entry entry = Entry.FromString(line);
            if (entry != null)
                leaderboard.Add(entry);
        }

        // Keep it sorted
        leaderboard = leaderboard.OrderByDescending(e => e.score).Take(maxEntries).ToList();
        UpdateUI();
    }

    public void ClearLeaderboard()
    {
        leaderboard.Clear();
        SaveLeaderboard();
        UpdateUI();
        Debug.Log("Leaderboard cleared.");
    }

    public void ReInitialize()
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            Debug.LogWarning("Canvas not found.");
            tmpText = null;
            return;
        }

        Transform display = canvasObj.transform.Find("LeaderboardCanvas/LeaderboardDisplay");
        if (display == null)
        {
            Debug.LogWarning("LeaderboardDisplay not found.");
            tmpText = null;
            return;
        }

        tmpText = display.GetComponent<TextMeshProUGUI>();

        if (tmpText == null)
        {
            Debug.LogWarning("TextMeshProUGUI component missing.");
            return;
        }

        UpdateUI();
    }
}
