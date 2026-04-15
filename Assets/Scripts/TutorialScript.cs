using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public List<GameObject> Tutorialy = new List<GameObject>();
    public GameObject mainTutorialObject; // hlavný objekt (napr. celý panel)

    public PauseMenu pauseMenu;

    private int currentIndex = 0;

    public AchievementSystem achievementSystem;


    void Start()
    {
        // Vypne všetky a zapne prvý
        for (int i = 0; i < Tutorialy.Count; i++)
        {
            Tutorialy[i].SetActive(i == 0);
        }
        PlayerController.SwitchingCameraMovement();
        pauseMenu.enabled = false;
    }

    public void ChangeTutorials()
    {
        // vypni aktuálny
        Tutorialy[currentIndex].SetActive(false);

        currentIndex++;

        // ak sme na konci
        if (currentIndex >= Tutorialy.Count)
        {
            if (mainTutorialObject != null)
                mainTutorialObject.SetActive(false);
            PlayerController.SwitchingCameraMovement();
            pauseMenu.enabled = true;
            achievementSystem.ShowAchievement(0);
            return;
        }

        // zapni ïalší
        Tutorialy[currentIndex].SetActive(true);
    }
}