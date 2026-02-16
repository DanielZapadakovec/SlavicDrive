using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public Text loadingText;

    [Header("Loading Tips")]
    public string[] loadingMessages = {
        "Loading CAR...",
        "Generating World...",
        "Inicializing Controls...",
        "Joining bottles to box...",
        "Heating motor..."
    };

    void Start()
    {
        StartCoroutine(LoadGameAsync());
    }

    IEnumerator LoadGameAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            progressBar.value = operation.progress;
            loadingText.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
            yield return new WaitForSeconds(0.5f);
        }

        progressBar.value = 1f;
        loadingText.text = "Naèítavam...";

        yield return new WaitForSeconds(1f);
        operation.allowSceneActivation = true;
    }
}
