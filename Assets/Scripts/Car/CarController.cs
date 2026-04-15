using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    // === Exposed values for camera & effects ===
    public float CurrentMotorTorque { get; private set; }
    public float MaxMotorTorque => baseMotorForce * GetMaxGearMultiplier();
    public bool IsShifting => isShifting;
    private InputActions controls;

    private float steering;
    public float acceleration;
    public float brake;
    private bool isHandBraking;

    private float currentSteerAngle, currentBrakeForce;
    public int currentGear = 1;
    public float currentSpeed;

    [Header("RPM_Props")]
    private float engineRPM;
    [SerializeField] private float idleRPM = 900f;
    [SerializeField] private float maxEngineRPM = 7000f;

    [Header("CarProps")]
    [SerializeField] private float baseMotorForce = 500f;
    [SerializeField] private float brakeForce = 1500f;
    [SerializeField] private float motorBrakeForce = 15f;
    [SerializeField] private float handBrakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float shiftDelay = 0.3f;
    [SerializeField] private float steeringSpeed = 5f;
    [SerializeField] private float minSteerSpeedEffect = 20f;
    [SerializeField] private float maxForwardSpeed = 160f;
    private bool isShifting = false;

    [Header("WheelProps")]
    [SerializeField] private float[] gearSpeeds = { 0f, 20f, 40f, 70f, 110f, 160f };
    [SerializeField] private float reverseMaxSpeed = 15f;
    [SerializeField] private float[] gearPowerMultipliers = { 1.2f, 0f, 1.5f, 1.2f, 0.8f, 0.6f };

    [Space(20)]
    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;
    [Space(20)]

    [Header("Wheels")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    [Space(20)]

    [Header("Steering Wheel")]
    [SerializeField] private Transform steeringWheelTransform;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource reverseSound;
    [SerializeField] private AudioSource shiftingSound;
    [SerializeField] private AudioClip shiftUpSound;
    [SerializeField] private AudioClip shiftDownSound;

    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;

    [Header("DashBoardThings")]
    [SerializeField] private Transform SpeedometerNeedle;
    [SerializeField] private Transform RPMNeedle;
    [SerializeField] private Transform FuelNeedle;

    private float minZRotation = 150f;
    private float maxZRotation = -80f;
    public float minZRotationRPM = 250f;
    public float maxZRotationRPM = -80f;
    public float minZRotationFuel = -70f;
    public float maxZRotationFuel = 70f;
    private float maxSpeed = 140f;
    [SerializeField] private float wheelRadius = 0.34f;
    [SerializeField] private float finalDriveRatio = 3.9f;

    [SerializeField] private Text gearText;

    private Rigidbody carRigidbody;
    public CarInteractables carInteractables;

    private float currentWheelRotation = 0f;
    [SerializeField] private float maxWheelRotation = 720f;
    [SerializeField] private float steeringWheelSpeed = 300f;
    [SerializeField] private float returnSpeed = 100f; 

    [Header("CarAssembly")]
    public CarAssembly carAssembly;

    #region Controls
    private void Awake()
    {
        controls = new InputActions();
    }

    private void OnEnable()
    {
        controls.Enable();

        // Steering
        controls.Car.Steer.performed += ctx => steering = ctx.ReadValue<float>();
        controls.Car.Steer.canceled += ctx => steering = 0f;

        // Acceleration (W)
        controls.Car.Accelerate.performed += ctx => acceleration = ctx.ReadValue<float>();
        controls.Car.Accelerate.canceled += ctx => acceleration = 0f;

        // Brake (S)
        controls.Car.Brake.performed += ctx => brake = ctx.ReadValue<float>();
        controls.Car.Brake.canceled += ctx => brake = 0f;

        // Handbrake
        controls.Car.Handbrake.performed += ctx => isHandBraking = true;
        controls.Car.Handbrake.canceled += ctx => isHandBraking = false;

        // Gear shifting
        controls.Car.UpShift.performed += ctx => ShiftGear(1);
        controls.Car.DownShift.performed += ctx => ShiftGear(-1);
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
    #region StartUpdate
    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        currentSpeed = carRigidbody.velocity.magnitude * 3.6f;

        if (carInteractables.isSeated && carInteractables.engineRunning)
        {
            HandleMotor();
            HandleBraking();
            if (carAssembly.IsPartInstalled(ItemType.SteeringWheel)) {HandleSteering(); }
            UpdateEngineSound();
            UpdateSpeedometer();
            UpdateGearText();
            UpdateRPMmeter();
            UpdateFuelMeter();
            carInteractables.DeFuelling();
            Slowdown();
        }
        else if (carInteractables.isSeated && !carInteractables.engineRunning)
        {
           if (carAssembly.IsPartInstalled(ItemType.SteeringWheel)) { HandleSteering(); }
            HandleBraking();
        }
        UpdateWheels();

        if (isHandBraking) ApplyHandBraking();
        GearShiftPulse = Mathf.MoveTowards(GearShiftPulse, 0f, Time.deltaTime * 4f);
    }
    #endregion
    private void HandleMotor()
    {
      float torque = baseMotorForce * acceleration * gearPowerMultipliers[currentGear];
    torque *= -1f;

         CurrentMotorTorque = Mathf.Abs(torque);
        if (currentGear > 1 && acceleration > 0)
        {
            float maxSpeedForGear = gearSpeeds[currentGear];
            if (currentSpeed < maxSpeedForGear)
            {
                frontLeftWheelCollider.motorTorque = torque;
                frontRightWheelCollider.motorTorque = torque;
            }
            else
            {
                frontLeftWheelCollider.motorTorque = 0f;
                frontRightWheelCollider.motorTorque = 0f;
            }
        }
        else if (currentGear == 0 && acceleration > 0) // Reverse
        {
            if (currentSpeed < reverseMaxSpeed)
            {
                frontLeftWheelCollider.motorTorque = -torque;
                frontRightWheelCollider.motorTorque = -torque;
            }
            else
            {
                frontLeftWheelCollider.motorTorque = 0f;
                frontRightWheelCollider.motorTorque = 0f;
            }
        }
        else
        {
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
        }
    }

    private void HandleBraking()
    {
        currentBrakeForce = brake * brakeForce;
        ApplyBraking();
    }

    private void ApplyHandBraking()
    {
        rearLeftWheelCollider.brakeTorque = handBrakeForce;
        rearRightWheelCollider.brakeTorque = handBrakeForce;
    }

    private void ApplyBraking()
    {
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void Slowdown()
    {
        if (carRigidbody != null && acceleration == 0 && brake == 0 && !isHandBraking)
        {
            carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, Vector3.zero, motorBrakeForce);
        }
    }

    private void HandleSteering()
    {
        UpdateSteeringWheel();
        float targetSteerAngle = maxSteerAngle * steering;
        float adjustedSpeedFactor = Mathf.InverseLerp(minSteerSpeedEffect, maxForwardSpeed, currentSpeed);
        float adjustedTurnAngle = targetSteerAngle * (1f - adjustedSpeedFactor);

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, adjustedTurnAngle, Time.deltaTime * steeringSpeed);

        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void UpdateSteeringWheel()
    {
        if (steeringWheelTransform == null) return;

        // cieľová rotácia podľa steering inputu
        float targetRotation = steering * maxWheelRotation;

        // plynulý prechod medzi aktuálnou a cieľovou rotáciou
        currentWheelRotation = Mathf.MoveTowards(
            currentWheelRotation,
            targetRotation,
            steeringWheelSpeed * Time.deltaTime
        );

        // ak nie je input (steering ~ 0), pomaly sa vracia do stredu
        if (Mathf.Abs(steering) < 0.01f)
        {
            currentWheelRotation = Mathf.MoveTowards(
                currentWheelRotation,
                0f,
                returnSpeed * Time.deltaTime
            );
        }

        // aplikuj rotáciu na volant (len okolo osi Z alebo Y podľa modelu)
        steeringWheelTransform.localRotation = Quaternion.Euler(0f, -currentWheelRotation, 0);
    }

    private void UpdateEngineRPM()
    {
        float wheelRPM = (carRigidbody.velocity.magnitude / (2f * Mathf.PI * wheelRadius)) * 60f;
        float gearRatio = gearPowerMultipliers[currentGear] * finalDriveRatio;
        float speedBasedRPM = Mathf.Abs(wheelRPM * gearRatio);
        float throttleRPM = idleRPM + Mathf.Abs(acceleration) * (maxEngineRPM - idleRPM);

        if (currentGear == 1 || carRigidbody.velocity.magnitude < 0.5f)
        {
            engineRPM = Mathf.Lerp(engineRPM, throttleRPM, Time.deltaTime * 5f);
        }
        else
        {
            float targetRPM = Mathf.Max(speedBasedRPM, throttleRPM * 0.4f);
            engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.deltaTime * 5f);
        }

        engineRPM = Mathf.Clamp(engineRPM, idleRPM, maxEngineRPM);
    }

    private void UpdateEngineSound()
    {
        UpdateEngineRPM();
        if (engineSound == null) return;

        float rpmFactor = Mathf.Lerp(minPitch, maxPitch, engineRPM / maxEngineRPM);
        float maxSpeedForGear = (currentGear > 1 && currentGear < gearSpeeds.Length) ? gearSpeeds[currentGear] : maxForwardSpeed;
        float speedFactor = Mathf.Lerp(0.8f, 1.2f, currentSpeed / maxSpeedForGear);

        float pitch = rpmFactor * speedFactor;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (currentGear == 0) // Reverse
        {
            if (!reverseSound.isPlaying)
            {
                engineSound.Stop();
                reverseSound.Play();
            }
            reverseSound.pitch = pitch;
        }
        else
        {
            if (!engineSound.isPlaying)
            {
                reverseSound.Stop();
                engineSound.Play();
            }
            engineSound.pitch = pitch;
        }
    }

    public float GearShiftPulse { get; private set; }
    private void ShiftGear(int direction)
    {
        if (carInteractables.isSeated)
        {
            int newGear = currentGear + direction;
            if (newGear >= 0 && newGear < gearSpeeds.Length && !isShifting)
            {
                StartCoroutine(GearShiftRoutine(newGear, direction));
            }
        }
    }

    private IEnumerator GearShiftRoutine(int newGear, int direction)
    {
        isShifting = true;
        GearShiftPulse = 1f;   // <-- pulse start
        engineSound.pitch = minPitch;

        if (direction > 0 && shiftUpSound != null) shiftingSound.PlayOneShot(shiftUpSound);
        if (direction < 0 && shiftDownSound != null) shiftingSound.PlayOneShot(shiftDownSound);

        yield return new WaitForSeconds(shiftDelay);

        currentGear = newGear;
        isShifting = false;
    }

    private void UpdateSpeedometer()
    {
        if (carRigidbody == null || SpeedometerNeedle == null) return;

        float currentSpeed = carRigidbody.velocity.magnitude * 3.6f;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        float rotationZ = Mathf.Lerp(minZRotation, maxZRotation, currentSpeed / maxSpeed);
        SpeedometerNeedle.localRotation = Quaternion.Euler(SpeedometerNeedle.localRotation.x, SpeedometerNeedle.localRotation.y, rotationZ);
    }

    private void UpdateRPMmeter()
    {
        if (carRigidbody == null || RPMNeedle == null) return;

        float rotationZ = Mathf.Lerp(minZRotationRPM, maxZRotationRPM, engineRPM / maxEngineRPM);
        RPMNeedle.localRotation = Quaternion.Euler(RPMNeedle.localRotation.x, RPMNeedle.localRotation.y, rotationZ);
    }

    private void UpdateFuelMeter()
    {
        float rotationZFuel = Mathf.Lerp(minZRotationFuel, maxZRotationFuel, carInteractables.fuelLevel / 100);
        FuelNeedle.localRotation = Quaternion.Euler(0, 0, rotationZFuel);
    }

    private void UpdateGearText()
    {
        if (gearText != null)
        {
            switch (currentGear)
            {
                case 0:
                    gearText.text = "R";
                    break;
                case 1:
                    gearText.text = "N";
                    break;
                default:
                    gearText.text = (currentGear - 1).ToString();
                    break;
            }
        }
    }
    private float GetMaxGearMultiplier()
    {
        float max = 0f;
        foreach (float g in gearPowerMultipliers)
            if (g > max) max = g;
        return max;
    }
}
