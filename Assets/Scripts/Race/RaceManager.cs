using UnityEngine;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public List<GameObject> checkpoints;
    public int currentCheckpoint = 0;

    public bool isRegistered = false;
    public bool raceOngoing = false;
    public bool raceFinished = false;

    private float raceTime = 0f;

    public List<float> npcTimes; // manu·lne nastavÌö v inspectore
    public string playerResult = "";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideAllCheckpoints();
    }

    private void Update()
    {
        if (raceOngoing)
        {
            raceTime += Time.deltaTime;
        }
    }

    public void RegisterPlayer()
    {
        if (!isRegistered)
        {
            isRegistered = true;
            Debug.Log("Hr·Ë sa zapÌsal na z·vod!");
        }
    }

    public void StartRace()
    {
        if (!isRegistered || raceOngoing) return;

        raceOngoing = true;
        raceFinished = false;
        raceTime = 0f;
        currentCheckpoint = 0;

        ActivateCheckpoint(0);

        Debug.Log("Z·vod zaËal!");
    }

    public void FinishRace()
    {
        raceOngoing = false;
        raceFinished = true;

        playerResult = FormatTime(raceTime);

        ShowResults();
        Debug.Log("Z·vod dokonËen˝!");
    }

    public void Disqualify()
    {
        raceOngoing = false;
        isRegistered = false;
        playerResult = "DISKVALIFIKOVAN›";

        HideAllCheckpoints();
        ShowResults();
    }

    public void NextCheckpoint()
    {
        currentCheckpoint++;

        if (currentCheckpoint >= checkpoints.Count)
        {
            FinishRace();
        }
        else
        {
            ActivateCheckpoint(currentCheckpoint);
        }
    }

    void ActivateCheckpoint(int index)
    {
        HideAllCheckpoints();
        checkpoints[index].SetActive(true);
    }

    void HideAllCheckpoints()
    {
        foreach (var cp in checkpoints)
            cp.SetActive(false);
    }

    public void ResetRaceState()
    {
        raceOngoing = false;
        raceFinished = false;
        isRegistered = false;
        HideAllCheckpoints();
    }

    void ShowResults()
    {
        // v˝sledky vloûÌme do listu, aby sme ich mohli zoradiù
        List<RaceResult> results = new List<RaceResult>();

        // hr·Ë (ak nie diskvalifikovan˝)
        bool disq = playerResult == "DISKVALIFIKOVAN›";

        if (!disq)
        {
            results.Add(new RaceResult("Player", raceTime));
        }
        else
        {
            // diskvalifikovan˝ -> öpeci·lny z·znam
            results.Add(new RaceResult("Player (DQ)", float.MaxValue));
        }

        // NPC Ëasy
        for (int i = 0; i < npcTimes.Count; i++)
        {
            results.Add(new RaceResult("NPC " + (i + 1), npcTimes[i]));
        }

        // ZORADENIE PODºA »ASU
        results.Sort((a, b) => a.time.CompareTo(b.time));

        // Poskladanie textu pre UI
        string output = "";

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].time == float.MaxValue)
            {
                output += (i + 1) + ". " + results[i].name + " - DISQUALIFIED\n";
            }
            else
            {
                output += (i + 1) + ". " + results[i].name + " - " + FormatTime(results[i].time) + "\n";
            }
        }

        // poslaù do UI
        RaceUIManager.Instance.ShowResults(output);
    }

    string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60);
        float seconds = t % 60f;
        return minutes.ToString("00") + ":" + seconds.ToString("00.00");
    }
    public struct RaceResult
    {
        public string name;
        public float time;

        public RaceResult(string name, float time)
        {
            this.name = name;
            this.time = time;
        }
    }
}