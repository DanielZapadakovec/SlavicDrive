using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePlayer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject continueHint; // napr. "Press E" ikonka (optional)

    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Typing")]
    [SerializeField] private float defaultSecondsPerCharacter = 0.03f;

    [Header("Input")]
    [SerializeField] private KeyCode continueKey = KeyCode.E;
    [SerializeField] private bool mouseClickContinues = true;

    [Header("Events")]
    public UnityEvent onDialogueStarted;
    public UnityEvent onDialogueFinished;

    public bool isTyping;
    private Coroutine playRoutine;
    private bool waitingForContinue;
    private bool lineFullyRevealed;
    private DialogueAsset currentAsset;

    private void Awake()
    {
        if (continueHint) continueHint.SetActive(false);
        if (voiceSource == null) voiceSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentAsset == null) return;

        if (waitingForContinue && GetContinuePressed())
        {
            waitingForContinue = false;
        }
        else if (!waitingForContinue && !lineFullyRevealed && GetContinuePressed())
        {
            // skip typewriter -> okamžite dopíše celý riadok
            lineFullyRevealed = true;
        }
    }

    private bool GetContinuePressed()
    {
        if (Input.GetKeyDown(continueKey)) return true;
        if (mouseClickContinues && Input.GetMouseButtonDown(0)) return true;
        return false;
    }

    public void Play(DialogueAsset asset)
    {
        if (asset == null) return;
        if (!asset.CanPlay()) return;

        // stop current
        Stop();

        currentAsset = asset;
        playRoutine = StartCoroutine(PlayRoutine(asset));
        isTyping = true;
    }

    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        if (voiceSource) voiceSource.Stop();

        currentAsset = null;
        waitingForContinue = false;
        lineFullyRevealed = false;

        if (continueHint) continueHint.SetActive(false);
        isTyping = false;
    }

    private IEnumerator PlayRoutine(DialogueAsset asset)
    {
        onDialogueStarted?.Invoke();

        for (int i = 0; i < asset.lines.Count; i++)
        {
            DialogueLine line = asset.lines[i];
            yield return StartCoroutine(PlayLine(line));
        }

        // play-once flag: nastav až po komplet dohraní
        if (asset.playOnlyOnce)
            asset.hasPlayed = true;

        onDialogueFinished?.Invoke();
        Stop(); // schová UI a zresetuje
    }

    private IEnumerator PlayLine(DialogueLine line)
    {
        if (continueHint) continueHint.SetActive(false);

        // audio
        if (voiceSource)
        {
            voiceSource.Stop();
            if (line.voice != null)
            {
                voiceSource.clip = line.voice;
                voiceSource.Play();
            }
        }

        // typewriter
        dialogueText.text = "";
        string full = line.text ?? "";
        float spc = (line.secondsPerCharacter > 0f) ? line.secondsPerCharacter : defaultSecondsPerCharacter;

        lineFullyRevealed = false;

        int idx = 0;
        while (idx < full.Length)
        {
            if (lineFullyRevealed)
            {
                dialogueText.text = full;
                break;
            }

            idx++;
            dialogueText.text = full.Substring(0, idx);
            yield return new WaitForSeconds(spc);
        }

        // po dopísaní
        yield return new WaitForSeconds(line.extraHoldTime);

        // Auto pokraèovanie (ak waitForContinue = false)
        if (!line.waitForContinue)
        {
            float hold = 0.2f;

            // ak je audio, poèkaj kým dohrá (alebo aspoò približne)
            if (line.voice != null)
            {
                // hooking na isPlaying je OK pre voiceSource
                while (voiceSource != null && voiceSource.isPlaying)
                    yield return null;
            }
            else
            {
                // keï nie je audio, nechaj krátku pauzu pod¾a dåžky textu
                hold = Mathf.Clamp(full.Length * 0.02f, 0.2f, 2.0f);
                yield return new WaitForSeconds(hold);
            }
            yield break;
        }

        // Èakaj na continue input
        waitingForContinue = true;
        if (continueHint) continueHint.SetActive(true);

        while (waitingForContinue)
            yield return null;

        waitingForContinue = false;
        if (continueHint) continueHint.SetActive(false);
    }
}
