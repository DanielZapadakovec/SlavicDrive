using UnityEngine;

public class FuelPumpHandler : MonoBehaviour
{
    public Transform fuelPumpStation;
    public Transform playerHand;
    public GameObject fuelNozzle;
    public GameObject noozleCollider;
    public float returnDistance = 5f;

    [HideInInspector]public bool isHoldingNozzle = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private BoxCollider nozzleCollider;

    void Start()
    {
        originalPosition = fuelNozzle.transform.position;
        originalRotation = fuelNozzle.transform.rotation;

    }

    void Update()
    {
        if (isHoldingNozzle && Vector3.Distance(playerHand.position, fuelPumpStation.position) > returnDistance)
        {
            ReturnNozzleToStation();
        }
    }

    public void TakeNozzle()
    {
        if (!isHoldingNozzle)
        {
            isHoldingNozzle = true;
            fuelNozzle.transform.SetParent(playerHand);
            fuelNozzle.transform.localPosition = Vector3.zero;
            fuelNozzle.transform.localRotation = Quaternion.identity;

            noozleCollider.SetActive(true);
        }
    }

    public void ReturnNozzleToStation()
    {
        if (isHoldingNozzle)
        {
            isHoldingNozzle = false;

            fuelNozzle.transform.SetParent(null);
            fuelNozzle.transform.position = originalPosition;
            fuelNozzle.transform.rotation = originalRotation;
            noozleCollider.SetActive(false);

        }
    }
}
