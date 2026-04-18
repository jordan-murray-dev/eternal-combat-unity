using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject pauseCanvas;
    public GameObject controlsCanvas;

    private bool isPaused = false;
    private Keyboard keyboard;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        keyboard = Keyboard.current;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (controlsCanvas != null) controlsCanvas.SetActive(false);
    }

    void Update()
    {
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isPaused && controlsCanvas.activeSelf)
            {
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (controlsCanvas != null) controlsCanvas.SetActive(false);
        
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (controlsCanvas != null) controlsCanvas.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenControls()
    {
        if (controlsCanvas != null) controlsCanvas.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        if (controlsCanvas != null) controlsCanvas.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
