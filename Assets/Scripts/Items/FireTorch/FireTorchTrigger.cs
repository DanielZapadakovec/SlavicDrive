using UnityEngine;

public class FireTorchTrigger : MonoBehaviour
{
    public FireTorch torch; // odkaz na FireTorch skript

    private void OnTriggerStay(Collider other)
    {
        if (torch.isActive && other.CompareTag("HeaterMachine"))
        {
            HeaterMachine heater = other.GetComponent<HeaterMachine>();
            if (heater != null)
            {
                heater.Ignite();
            }
        }
    }
}