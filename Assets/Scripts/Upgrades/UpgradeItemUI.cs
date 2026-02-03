using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text priceText;
    public Button buyButton;
    public GameObject boughtOverlay;

    private UpgradeData data;
    private PlayerStatsSystem stats;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Setup(UpgradeData upgrade)
    {
        data = upgrade;
        stats = FindAnyObjectByType<PlayerStatsSystem>();

        icon.sprite = data.icon;
        nameText.text = data.upgradeName;
        priceText.text = data.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Buy);

        Refresh();
    }

    void Buy()
    {
        if (data.isBought) return;
        if (stats.currentMoney < data.price) return;

        stats.AddMoney(-data.price);
        data.isBought = true;
        audioSource.Play();

        UpgradeApplier.Instance.Apply(data);

        Refresh();
    }

    void Refresh()
    {
        buyButton.interactable = !data.isBought;
        boughtOverlay.SetActive(data.isBought);
    }
}
