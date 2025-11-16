using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RaceUIManager : MonoBehaviour
{
    public static RaceUIManager Instance;

    public Text titleText;
    public Text resultsText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowRegistration()
    {
        gameObject.SetActive(true);
        titleText.text = "Race Registration";
        resultsText.text = "";
    }

    public void ShowRegistered()
    {
        gameObject.SetActive(true);
        titleText.text = "Registered!";
        resultsText.text = "Wait in the starting zone to begin the race.";
    }

    public void ShowResults(string finalList)
    {
        gameObject.SetActive(true);
        titleText.text = "Finish List";
        resultsText.text = finalList;
    }
}
