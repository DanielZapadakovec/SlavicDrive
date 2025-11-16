using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Car") && RaceManager.Instance.raceOngoing)
        {
            RaceManager.Instance.NextCheckpoint();
        }
    }
}
