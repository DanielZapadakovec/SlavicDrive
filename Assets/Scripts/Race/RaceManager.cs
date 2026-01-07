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

    public List<float> npcTimes; // manuálne nastavíš v inspectore
    public string playerResult = "";

    public GameObject raceEntryZone;
    public PlayerStatsSystem playerStatsSystem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideAllCheckpoints();
        raceEntryZone.SetActive(false);
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
            raceEntryZone.SetActive(true);
            RaceUIManager.Instance.ShowRegistered();
        }
    }

    public void StartRace()
    {
        if (!isRegistered || raceOngoing) return;

        raceOngoing = true;
        raceFinished = false;
        raceTime = 0f;
        currentCheckpoint = 0;

        raceEntryZone.SetActive(false);
        ActivateCheckpoint(0);

        Debug.Log("Závod zaèal!");
    }

    public void FinishRace()
    {
        raceOngoing = false;
        raceFinished = true;

        playerResult = FormatTime(raceTime);

        ShowResults();
        Debug.Log("Závod dokonèený!");
    }

    public void Disqualify()
    {
        raceOngoing = false;
        isRegistered = false;
        playerResult = "DISKVALIFIKOVANÝ";

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
        HideAllCheckpoints();
        List<RaceResult> results = new List<RaceResult>();

        bool disq = playerResult == "DISKVALIFIKOVANÝ";

        if (!disq)
        {
            results.Add(new RaceResult("Player", raceTime));
            playerStatsSystem.AddMoney(1000);
        }
        else
        {
            results.Add(new RaceResult("Player (DQ)", float.MaxValue));
        }

        // NPC èasy
        for (int i = 0; i < npcTimes.Count; i++)
        {
            results.Add(new RaceResult("NPC " + (i + 1), npcTimes[i]));
        }

        results.Sort((a, b) => a.time.CompareTo(b.time));

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

        // posla do UI
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