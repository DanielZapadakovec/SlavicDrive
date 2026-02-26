using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject settingsUI;
    public MonoBehaviour[] scriptsToDisable;
    private bool isPaused = false;
    public GameObject playerAudioSource;
    public GameObject buttons;
    bool settingsOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !StorageInventory.isOpen )
        {
            if (isPaused)
            {
                if (settingsOpen)
                {
                    Settings();
                    
                }
                else
                    Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        SetScriptsEnabled(true);
        isPaused = false;
        settingsUI.SetActive(false);
        PlayerController.SwitchingCameraMovement();
        playerAudioSource.SetActive(true);
        buttons.SetActive(false);
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        pauseUI.GetComponent<Animator>().Play("PauseMenuFade");
        SetScriptsEnabled(false);
        isPaused = true;
        PlayerController.SwitchingCameraMovement();
        playerAudioSource.SetActive(false);
        buttons.SetActive(true);
    }
    public void Settings()
    {
        if (!settingsOpen)
        {
            buttons.SetActive(false);
            settingsOpen = true;
            settingsUI.SetActive(true);
        }
        else if (settingsOpen)
        {
            buttons.SetActive(true);
            settingsOpen = false;
            settingsUI.SetActive(false);
        }
    }

    private void SetScriptsEnabled(bool enabled)
    {
        foreach (var script in scriptsToDisable)
        {
            script.enabled = enabled;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
