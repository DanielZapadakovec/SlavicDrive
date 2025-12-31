using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public CarController carController;

    [Header("Look Settings")]
    public float sensitivity = 2f;
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;

    private float yaw;
    private float pitch;

    [Header("FOV Settings")]
    public float baseFOV = 60f;
    public float maxThrottleFOVBoost = 12f;
    public float brakeFOVReduction = 6f;
    public float gearShiftFOVPulse = 6f;
    public float fovSmoothTime = 0.25f;

    private float targetFOV;
    private float fovVelocity;

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        cam.fieldOfView = baseFOV;
        targetFOV = baseFOV;

        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleFOV();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleFOV()
    {
        if (carController == null || cam == null)
            return;

        // N (neutral) ? žiadny FOV efekt
        if (carController.currentGear <= 1)
        {
            targetFOV = baseFOV;
        }
        else
        {
            // --- motor torque ? 0–1
            float torque01 = 0f;
            if (carController.MaxMotorTorque > 0f)
            {
                torque01 = Mathf.Clamp01(
                    carController.CurrentMotorTorque / carController.MaxMotorTorque
                );
            }

            float throttleBoost = torque01 * maxThrottleFOVBoost;

            float brakeEffect = 0f;
            if (carController.brake > 0.1f)
                brakeEffect = -brakeFOVReduction * carController.brake;

            float gearPulse = carController.GearShiftPulse * gearShiftFOVPulse;

            targetFOV = baseFOV + throttleBoost + brakeEffect + gearPulse;
        }

        cam.fieldOfView = Mathf.SmoothDamp(
            cam.fieldOfView,
            targetFOV,
            ref fovVelocity,
            fovSmoothTime
        );
    }
}
