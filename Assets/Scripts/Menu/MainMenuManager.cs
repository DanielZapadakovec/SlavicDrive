using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    #region [Properties] StartButton
    public PlayableDirector startDirector;
    #endregion
    #region [Properties] SettingsButton
    public GameObject SettingsPanel;
    public GameObject Buttons;
    bool settingsOpen;
    #endregion
    #region [Properties] QuitButton
    public PlayableDirector quitDirector;
    #endregion
    #region [Methods] StartButton
    public void StartDirector()
    {
        startDirector.Play();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }
    #endregion
    #region [Methods] SettingsButton

    public void SettingsOpen()
    {
        SettingsPanel.SetActive(true);
        Buttons.SetActive(false);
        settingsOpen = true;
    }
    public void SettingsClose()
    {
        SettingsPanel.SetActive(false);
        Buttons.SetActive(true);
        settingsOpen = false;
    }
    #endregion
    #region [Methods] QuitButton
    public void QuitDirector()
    {
        quitDirector.Play();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
    #region [Methods] Unity
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsOpen)
        {
            SettingsClose();
        }
    }
    #endregion
}
