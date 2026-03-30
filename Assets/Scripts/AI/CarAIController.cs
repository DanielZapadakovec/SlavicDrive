using System.Collections;
using UnityEngine;

public class CarAIController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Driving Settings")]
    public float maxSteerAngle = 30f;
    public float maxMotorTorque = 1500f;
    public float brakeForce = 3000f;

    [Header("Engine Sound")]
    public AudioSource engineSound;
    public AudioSource brakeSound;

    public float idlePitch = 0.8f;
    public float maxPitch = 1.6f;
    public float pitchResponse = 2.5f;

    [Header("Fake Gearbox")]
    public float[] gearSpeedLimits = { 0f, 15f, 35f, 60f, 90f };
    public float shiftPitchDrop = 0.25f;
    public float shiftDuration = 0.25f;

    [Header("Diesel Lag")]
    public float throttleResponseTime = 0.6f;
    private float smoothedThrottle;

    private Rigidbody rb;
    private float motorInput;
    private bool isBraking;

    private int currentGear = 1;
    private bool isShifting;
    private float targetPitch;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (engineSound != null)
            engineSound.Play();
    }

    private void FixedUpdate()
    {
        smoothedThrottle = Mathf.Lerp(
    smoothedThrottle,
    motorInput,
    Time.fixedDeltaTime / throttleResponseTime
    );
        HandleMotor();
        HandleAutoGears();
        UpdateEngineSound();
        UpdateBrakeSound();
    }

    // ================= AI INPUT =================

    public void SetSteer(float steerInput)
    {
        float steer = steerInput * maxSteerAngle;
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;
    }

    public void SetMotor(float motor)
    {
        motorInput = Mathf.Clamp01(motor);
        float torque = motorInput * maxMotorTorque;

        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    public void Brake(bool brake)
    {
        isBraking = brake;
        float force = brake ? brakeForce : 0f;

        frontLeft.brakeTorque = force;
        frontRight.brakeTorque = force;
        rearLeft.brakeTorque = force;
        rearRight.brakeTorque = force;
    }

    // ================= ENGINE SOUND =================

    private void UpdateEngineSound()
    {
        if (engineSound == null) return;

        float speed = rb.velocity.magnitude * 3.6f;
        float gearTopSpeed = gearSpeedLimits[Mathf.Min(currentGear, gearSpeedLimits.Length - 1)];
        float gearRatio = Mathf.Clamp01(speed / Mathf.Max(gearTopSpeed, 1f));

        targetPitch = Mathf.Lerp(idlePitch, maxPitch, gearRatio);

        if (isBraking)
            targetPitch *= 0.85f; // engine load while braking

        engineSound.pitch = Mathf.Lerp(
            engineSound.pitch,
            targetPitch,
            Time.fixedDeltaTime * pitchResponse
        );
    }

    // ================= BRAKE SOUND =================

    private void UpdateBrakeSound()
    {
        if (brakeSound == null) return;

        if (isBraking && rb.velocity.magnitude > 2f)
        {
            if (!brakeSound.isPlaying)
                brakeSound.Play();

            brakeSound.volume = Mathf.Lerp(
                brakeSound.volume,
                1f,
                Time.fixedDeltaTime * 4f
            );
        }
        else
        {
            brakeSound.volume = Mathf.Lerp(
                brakeSound.volume,
                0f,
                Time.fixedDeltaTime * 4f
            );

            if (brakeSound.volume < 0.05f)
                brakeSound.Stop();
        }
    }

    // ================= FAKE AUTO GEARBOX =================

    private void HandleAutoGears()
    {
        if (isShifting) return;

        float speed = rb.velocity.magnitude * 3.6f;

        if (currentGear < gearSpeedLimits.Length - 1 &&
            speed > gearSpeedLimits[currentGear])
        {
            StartCoroutine(ShiftGear(currentGear + 1));
        }
        else if (currentGear > 1 &&
                 speed < gearSpeedLimits[currentGear - 1] - 5f)
        {
            StartCoroutine(ShiftGear(currentGear - 1));
        }
    }

    private IEnumerator ShiftGear(int newGear)
    {
        isShifting = true;

        float originalPitch = engineSound.pitch;
        engineSound.pitch = Mathf.Max(idlePitch, originalPitch - shiftPitchDrop);

        yield return new WaitForSeconds(shiftDuration);

        currentGear = newGear;
        isShifting = false;
    }
    
    // ================= WHEELS =================

    private void Update()
    {
        UpdateLeftWheel(frontLeft, frontLeftMesh);
        UpdateRightWheel(frontRight, frontRightMesh);
        UpdateLeftWheel(rearLeft, rearLeftMesh);
        UpdateRightWheel(rearRight, rearRightMesh);
    }

    private void UpdateLeftWheel(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        mesh.position = pos;
        mesh.rotation = rot *= Quaternion.Euler(0f, -90f, 0f);
    }
    private void UpdateRightWheel(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);


        mesh.position = pos;
        mesh.rotation = rot *= Quaternion.Euler(0f, 90f, 0f);
    }


    private void HandleMotor()
    {
        if (isBraking)
        {
            rearLeft.motorTorque = 0f;
            rearRight.motorTorque = 0f;
            return;
        }

        float speed = rb.velocity.magnitude * 3.6f;

        float throttleCurve = Mathf.Lerp(0.3f, 1f, smoothedThrottle);

        float speedLimiter = 1f;
        if (currentGear < gearSpeedLimits.Length)
        {
            float gearTopSpeed = gearSpeedLimits[currentGear];
            speedLimiter = Mathf.Clamp01(1f - (speed / (gearTopSpeed + 5f)));
        }

        float torque =
            maxMotorTorque *
            throttleCurve *
            speedLimiter;

        // aplikácia na zadnú nápravu (typické pre bus)
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;

        // mierne motorové brzdenie pri ubraní plynu
        if (motorInput < 0.05f && speed > 5f)
        {
            rearLeft.brakeTorque = brakeForce * 0.15f;
            rearRight.brakeTorque = brakeForce * 0.15f;
        }
        else
        {
            rearLeft.brakeTorque = 0f;
            rearRight.brakeTorque = 0f;
        }
    }

}
