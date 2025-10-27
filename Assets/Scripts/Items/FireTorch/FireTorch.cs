using UnityEngine;

public class FireTorch : MonoBehaviour
{
    public GameObject fireParticles;
    public ParticleSystem fireParticleSystem;
    public float fireEnergy = 100f;
    public float energyConsumptionRate = 10f;
    public ItemGrabber grabObject;
    public bool isActive = false;

    void Update()
    {
       /* if (grabObject.isInteractingWithItem)
        {
            ToggleFire(true);

        }

        if (!grabObject.isInteractingWithItem)
        {
            ToggleFire(false);
        }*/

        if (isActive)
        {
            fireEnergy -= energyConsumptionRate * Time.deltaTime;
            fireEnergy = Mathf.Clamp(fireEnergy, 0, 100f);
            if (fireEnergy <= 0)
            {
                ToggleFire(false);
            }
        }

    }

    void ToggleFire(bool state)
    {
        isActive = state;
        if (state) fireParticles.SetActive(true);
        else fireParticles.SetActive(false);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("HeaterMachine"))
        {
            HeaterMachine heater = other.GetComponent<HeaterMachine>();
            if (heater != null)
            {
                heater.Ignite();
            }
        }
    }
}
