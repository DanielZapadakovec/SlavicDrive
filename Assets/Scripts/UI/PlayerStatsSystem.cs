using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.Playables;

public class PlayerStatsSystem : MonoBehaviour
{
    #region [Properties] MoneySystem
    [Header("Money Settings")]
    public float currentMoney = 0f;
    [SerializeField] private Text moneyText;
    #endregion

    #region [Properties] HungerSystem
    [Header("Hunger Settings")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private GameObject hungerWarning;
    [SerializeField] private float hungerIncreaseRate = 0.01f;
    public event Action OnHungerCritical;
    public float hunger = 0f;
    #endregion

    #region [Properties] ThirstSystem
    [Header("Thirst Settings")]
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private GameObject thirstWarning;
    [SerializeField] private float thirstIncreaseRate = 0.02f;
    public event Action OnThirstCritical;
    public float thirst = 0f;
    #endregion

    #region [Properties] FatigueSystem
    [Header("Fatigue Settings")]
    [SerializeField] private Slider fatigueSlider;
    [SerializeField] private GameObject fatigueWarning;
    [SerializeField] private float fatigueIncreaseRate = 0.005f;
    public event Action OnFatigueCritical;
    public float fatigue = 0f;
    #endregion
    #region [Properties] DayNightCycle
    [Header("DayNightCycle Settings")]
    public DayNightCycle dayNightCycle;
    #endregion

    public UnityEvent OnDeath;
    private bool isDead = false;

    #region [Methods] Unity
    void Start()
    {
        UpdateMoneyText();
        hungerWarning.gameObject.SetActive(false);
        thirstWarning.gameObject.SetActive(false);
        fatigueWarning.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        IncreaseStat(ref hunger, hungerIncreaseRate, hungerSlider, hungerWarning, OnHungerCritical);
        IncreaseStat(ref thirst, thirstIncreaseRate, thirstSlider, thirstWarning, OnThirstCritical);
        IncreaseStat(ref fatigue, fatigueIncreaseRate, fatigueSlider, fatigueWarning, OnFatigueCritical);

        if (hunger >= 1f || thirst >= 1f || fatigue >= 1f)
        {
            Die();
        }
    }
    #endregion

    #region [Methods] DieSystem
    private void Die()
    {
        isDead = true;
        OnDeath.Invoke();
    }
    #endregion

    #region [Methods] MoneySystem
    public float GetMoney() => (float)Math.Round(currentMoney, 6);

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        currentMoney = (float)Math.Round(currentMoney, 2);
        UpdateMoneyText();
    }

    public bool SubtractMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            currentMoney = (float)Math.Round(currentMoney, 2);
            UpdateMoneyText();
            return true;
        }
        else
        {
            Debug.LogWarning("Nedostatok peňazí!");
            return false;
        }
    }

    public void SetMoney(float amount)
    {
        currentMoney = (float)Math.Round(amount, 2);
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (moneyText)
            moneyText.text = $"{currentMoney:0.00} €";
    }
    #endregion

    #region [Methods] Stats
    public void AddHunger(float amount) => hunger = Mathf.Clamp01(hunger + amount);
    public void AddThirst(float amount) => thirst = Mathf.Clamp01(thirst + amount);
    public void AddFatigue(float amount) => fatigue = Mathf.Clamp01(fatigue + amount);

    public void SetHunger(float value) => hunger = Mathf.Clamp01(value);
    public void SetThirst(float value) => thirst = Mathf.Clamp01(value);
    public void SetFatigue(float value) => fatigue = Mathf.Clamp01(value);
    private void IncreaseStat(ref float stat, float rate, Slider slider, GameObject warning, Action onCritical)
    {
        stat += rate * Time.deltaTime;
        stat = Mathf.Clamp01(stat);
        slider.value = stat;

        if (stat >= 0.9f)
            warning.gameObject.SetActive(true);
        else
            warning.gameObject.SetActive(false);

        if (stat >= 0.9f && onCritical != null)
            onCritical.Invoke();
    }
    public void Sleep()
    {
        dayNightCycle.StartSleepEffect(fatigue);
        SetFatigue(0);
    }
    #endregion
}
