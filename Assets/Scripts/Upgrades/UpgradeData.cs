using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public int price;

    public UpgradeType upgradeType;

    [Header("Generic Values")]
    public int intValue;
    public float floatValue;

    public bool isBought;

    public string Description;
}