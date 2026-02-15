using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    public static UpgradeApplier Instance;
    [Header("CarStorageUpgrade")]
    public StorageObject carStorageObject;
    public int newSlots;
    [Header("RoadMaterialChange")]
    public GameObject dirtRoad;
    public Material asphaltMaterial;
    [Header("CarMaterialChange")]
    public GameObject car;
    public Material newBodyPaintMaterial;
    [Header("UnlockLocalShop")]
    public GameObject localShopUpgradeables;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Apply(UpgradeData data)
    {
        switch (data.upgradeType)
        {
            case UpgradeType.IncreaseStorage:
                ApplyStorageUpgrade(data);
                break;

            case UpgradeType.ChangeMaterialRoad:
                ApplyMaterialChangeRoad(data);
                break;
            case UpgradeType.ChangeMaterialCar:
                ApplyMaterialChangeCar(data);
                break;
            case UpgradeType.UnlockLocalShop:
                UnlockLand(data);
                break;
        }
    }

    // =======================
    // INDIVIDUAL UPGRADES
    // =======================

    void ApplyStorageUpgrade(UpgradeData data)
    {
        carStorageObject.slotCount = newSlots;
    }

    void ApplyUnlockLand(UpgradeData data)
    {

    }

    void ApplyMaterialChangeRoad(UpgradeData data)
    {
        if (dirtRoad == null) return;

        var renderer = dirtRoad.GetComponent<Renderer>();
        if (renderer != null && asphaltMaterial != null)
            renderer.material = asphaltMaterial;
    }

    void ApplyMaterialChangeCar(UpgradeData data)
    {
        if (car == null) return;

        var renderer = car.GetComponent<Renderer>();
        if (renderer != null && newBodyPaintMaterial != null)
            renderer.material = newBodyPaintMaterial;
    }
    void UnlockLand(UpgradeData data)
    {
        localShopUpgradeables.SetActive(true);
    }


}
