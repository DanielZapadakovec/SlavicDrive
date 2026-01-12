using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [Header("Settings")]
    public float maxSteerAngle = 30f;
    public float maxMotorTorque = 1500f;
    public float brakeForce = 3000f;

    public void SetSteer(float steerInput)
    {
        float steer = steerInput * maxSteerAngle;
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;
    }

    public void SetMotor(float motorInput)
    {
        float torque = motorInput * maxMotorTorque;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    public void Brake(bool brake)
    {
        float force = brake ? brakeForce : 0f;
        frontLeft.brakeTorque = force;
        frontRight.brakeTorque = force;
        rearLeft.brakeTorque = force;
        rearRight.brakeTorque = force;
    }

    private void Update()
    {
        UpdateWheel(frontLeft, frontLeftMesh, false);
        UpdateWheel(frontRight, frontRightMesh, true);
        UpdateWheel(rearLeft, rearLeftMesh, false);
        UpdateWheel(rearRight, rearRightMesh, true);
    }

    private void UpdateWheel(WheelCollider col, Transform mesh, bool invertRotation)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        if (invertRotation)
            rot *= Quaternion.Euler(0f, 180f, 0f);

        mesh.position = pos;
        mesh.rotation = rot;
    }
}
