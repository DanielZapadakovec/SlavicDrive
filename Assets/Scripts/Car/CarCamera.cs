using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float sensitivity = 2f;
    public float minVerticalAngle = -60f, maxVerticalAngle = 60f;
    private float yaw = 0f;
    private float pitch = 0f;
    public Transform carTransform;

    [Header("Zoom Settings")]
    public float zoomFOV = 30f;
    public float defaultFOV = 60f;
    public float zoomSpeed = 10f;
    public float smoothZoomTime = 0.2f;
    public Camera cam;
    private float targetZoom;
    private float zoomVelocity = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * sensitivity;
        float vertical = Input.GetAxis("Mouse Y") * sensitivity;
        HandleZoom();
        yaw += horizontal;
        pitch -= vertical;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f) ;
    }
    private void HandleZoom()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            targetZoom = zoomFOV;
        }
        else
        {
            targetZoom = defaultFOV;
        }

        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetZoom, ref zoomVelocity, smoothZoomTime);
    }
}
