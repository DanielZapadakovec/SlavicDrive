using UnityEngine;

public class UpgradeScrollManager : MonoBehaviour
{
    public UpgradeData[] upgrades;
    public GameObject upgradePrefab;
    public Transform contentParent;

    void Start()
    {
        foreach (var upgrade in upgrades)
        {
            var ui = Instantiate(upgradePrefab, contentParent);
            ui.GetComponent<UpgradeItemUI>().Setup(upgrade);
        }
    }
}
