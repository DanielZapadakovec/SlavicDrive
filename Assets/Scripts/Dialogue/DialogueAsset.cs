using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Asset", fileName = "DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    [Header("Behaviour")]
    public bool playOnlyOnce = false;

    [Tooltip("Runtime flag. Nastaví sa na true po dohraní celého dialógu.")]
    public bool hasPlayed = false;

    [Header("Lines")]
    public List<DialogueLine> lines = new List<DialogueLine>();

    public bool CanPlay()
    {
        if (!playOnlyOnce) return true;
        return !hasPlayed;
    }
}

[Serializable]
public class DialogueLine
{
    [TextArea(2, 6)]
    public string text;

    [Tooltip("Volite¾nı dabing pre tento riadok.")]
    public AudioClip voice;

    [Tooltip("Rıchlos písania (sekundy na znak). Ak 0 alebo menej, pouije sa default z playera.")]
    public float secondsPerCharacter = 0.03f;

    [Tooltip("Ak true, poèká na input (Continue). Ak false, prejde ïalej po dobe pod¾a audio/textu.")]
    public bool waitForContinue = true;

    [Tooltip("Dodatoèná pauza po dopísaní textu (sekundy).")]
    public float extraHoldTime = 0.0f;
}