using UnityEngine;
using UnityEngine.UI;

public class LoanSystem : MonoBehaviour
{
    public PlayerStatsSystem stats;

    public int currentLoan;
    public float interestPercent = 5f;

    public int stepAmount = 50;
    public Text loanAmountText;
    public Text currentLoanText;

    private int pendingLoan;

    public void AddAmount()
    {
        pendingLoan += stepAmount;
        UpdateUI();
    }

    public void DecreaseAmount()
    {
        pendingLoan -= stepAmount;
        UpdateUI();
    }

    public void RemoveAmount()
    {
        pendingLoan = Mathf.Max(0, pendingLoan - stepAmount);
        UpdateUI();
    }

    public void TakeLoan()
    {
        if (pendingLoan <= 0) return;

        float interest = pendingLoan * (interestPercent / 100f);
        int finalLoan = Mathf.RoundToInt(pendingLoan + interest);

        currentLoan += finalLoan;
        stats.AddMoney(pendingLoan);

        pendingLoan = 0;
        UpdateUI();
    }

    public void PayLoan(int amount)
    {
        if (stats.currentMoney < amount) return;

        stats.AddMoney(-amount);
        currentLoan -= amount;
        currentLoan = Mathf.Max(0, currentLoan);

        UpdateUI();
    }

    void UpdateUI()
    {
        loanAmountText.text = pendingLoan.ToString();
        currentLoanText.text = "Loan: " + currentLoan;
    }
}
