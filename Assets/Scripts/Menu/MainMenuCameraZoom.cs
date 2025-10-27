using UnityEngine;

public class MainMenuCameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomRange = 1f;
    public float zoomSpeed = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * zoomSpeed, zoomRange);
        transform.localPosition = startPosition + new Vector3(0, 0, offset);
    }
}
