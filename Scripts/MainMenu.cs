using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EC;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas Screens")]
    public GameObject mainMenuCanvas;
    public GameObject leaderboardCanvas;
    public GameObject controlsCanvas;
    public GameObject playerCanvas;

    [Header("Player Name Inputs")]
    public TMP_InputField player1InputField;
    public TMP_InputField player2InputField;

    [Header("Warning Messages")]
    public TextMeshProUGUI player1WarningText1;
    public TextMeshProUGUI player1WarningText2;
    public TextMeshProUGUI player2WarningText1;
    public TextMeshProUGUI player2WarningText2;

    public float flashInterval = 0.3f;
    public int flashCount = 2;
    public float stayDuration = 3f;

    private Coroutine player1WarningRoutine1;
    private Coroutine player1WarningRoutine2;
    private Coroutine player2WarningRoutine1;
    private Coroutine player2WarningRoutine2;

    private void Start()
    {
        mainMenuCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);

        if(LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.ReInitialize();
        }

        player1WarningText1?.gameObject.SetActive(false);
        player1WarningText2?.gameObject.SetActive(false);
        player2WarningText1?.gameObject.SetActive(false);
        player2WarningText2?.gameObject.SetActive(false);
    }

    public void OnStartPressed()
    {
        mainMenuCanvas.SetActive(false);
        playerCanvas.SetActive(true);
    }

    public void OnPlayPressed()
    {
        string name1 = player1InputField.text.Trim();
        string name2 = player2InputField.text.Trim();

        bool valid = true;

        var issues1 = ValidateAllProblems(name1, name2);
        var issues2 = ValidateAllProblems(name2, name1);

        if (issues1.Count > 0)
        {
            ShowWarningsForPlayer(player1WarningText1, ref player1WarningRoutine1, player1WarningText2, ref player1WarningRoutine2, issues1);
            valid = false;
        }
        
        if(issues2.Count > 0)
        {
            ShowWarningsForPlayer(player2WarningText1, ref player2WarningRoutine1, player2WarningText2, ref player2WarningRoutine2, issues2);
            valid = false;
        }

        if (!valid) return;

        PlayerPrefs.SetString("Player1Name", name1);
        PlayerPrefs.SetString("Player2Name", name2);
        PlayerPrefs.Save();

        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScores();
        }

        mainMenuCanvas.SetActive(false);
        EC.App.ProcedureManager.ChangeProcedure(typeof(EC.Gameplay.ProcedureGameplay));
    }

    private List<string> ValidateAllProblems(string name, string otherName)
    {
        List<string> issues = new List<string>();

        if (string.IsNullOrEmpty(name))
        {
            issues.Add("Name cannont be empty!");
        }
        else
        {
            if (name.Length < 3)
            {
                issues.Add("Name must be at least 3 characters.");
            }
            
            foreach(char c in name)
            {
                if(!char.IsLetterOrDigit(c))
                {
                    issues.Add("Name can only contain letters or numbers.");
                    break;
                }
            }
        }

        if(!string.IsNullOrEmpty(name) && name.Equals(otherName, System.StringComparison.OrdinalIgnoreCase))
        {
            issues.Add("Names must be different!");
        }

        return issues;
    }

    private void ShowWarningsForPlayer(TextMeshProUGUI warningText1, ref Coroutine routine1, TextMeshProUGUI warningText2, ref Coroutine routine2, List<string> messages)
    {
        if (messages.Count > 0 && warningText1 != null)
        {
            if (routine1 != null)
            {
                StopCoroutine(routine1);
            }
            routine1 = StartCoroutine(FlashAndHoldWarning(warningText1, messages[0]));
        }

        if (messages.Count > 1 && warningText2 != null)
        {
            if (routine2 != null)
            {
                StopCoroutine(routine2);
            }
            routine2 = StartCoroutine(FlashAndHoldWarning(warningText2, messages[1]));
        }
    }
    
    private IEnumerator FlashAndHoldWarning(TextMeshProUGUI warningText, string message)
    {
        warningText.text = message;

        for (int i = 0; i < flashCount * 2; i++)
        {
            warningText.gameObject.SetActive(!warningText.gameObject.activeSelf);
            yield return new WaitForSeconds(flashInterval);
        }

        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(stayDuration);
        warningText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

    public void OnControlsPressed()
    {
        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
    }

    public void OnLeaderboardPressed()
    {
        mainMenuCanvas.SetActive(false);
        leaderboardCanvas.SetActive(true);
    }

    public void OnClearPressed()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.ClearLeaderboard();
        }
    }

    public void OnBackPressed()
    {
        controlsCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        playerCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void OnQuitPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator InitializeLeaderboard()
    {
        yield return null;
    }
}
