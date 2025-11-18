using UnityEngine;

public class RaceEntryZone : MonoBehaviour
{
    public float requiredTimeInside = 5f;  // hráè musí by v zóne 5 sekúnd
    private float timer = 0f;
    private bool playerInside = false;
    public Material zoneMaterial;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Car") && RaceManager.Instance.isRegistered)
        {
            playerInside = true;
            timer = 0f; // reset timeru pri vstupe
            zoneMaterial.color = Color.green;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            playerInside = false;
            timer = 0f; // reset keï odíde
        }
    }

    private void Update()
    {
        if (playerInside && RaceManager.Instance.isRegistered)
        {
            timer += Time.deltaTime; // tu pribúda èas

            if (timer >= requiredTimeInside)
            {
                RaceManager.Instance.StartRace();
                playerInside = false;
                zoneMaterial.color = Color.white;
            }
        }
    }
}