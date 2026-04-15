using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(CarAIController))]
[RequireComponent(typeof(Rigidbody))]
public class CarAIFollower : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline;
    [Range(0f, 1f)] public float t;

    [Header("Speed")]
    public float maxSpeed = 18f;
    public float minSpeed = 6f;

    [Header("Steering")]
    public float steeringSensitivity = 1.2f;
    public float lateralCorrectionStrength = 0.5f;

    [Header("Braking")]
    public float maxCurveAngle = 60f;
    public float brakeStrength = 1f;

    public bool loop;

    private CarAIController wheels;
    private Rigidbody rb;
    private bool blocked;

    private void Awake()
    {
        wheels = GetComponent<CarAIController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (spline == null || blocked)
        {
            wheels.SetMotor(0);
            wheels.Brake(true);
            return;
        }

        FollowSpline();
    }

    private void FollowSpline()
    {
        float splineLength = spline.CalculateLength();
        t += (rb.velocity.magnitude / splineLength) * Time.fixedDeltaTime;
        t = Mathf.Clamp01(t);

        if (t >= 1f)
        {
            if (loop) t = 0f;
            else
            {
                wheels.SetMotor(0);
                wheels.Brake(true);
                return;
            }
        }

        // spline data
        float3 tangent = spline.EvaluateTangent(t);
        Vector3 splineDir = math.normalize(new Vector3(tangent.x, tangent.y, tangent.z));
        Vector3 splinePos = spline.EvaluatePosition(t);

        // ==== HEADING ERROR ====
        float headingAngle = Vector3.SignedAngle(transform.forward, splineDir, Vector3.up);
        float headingSteer = headingAngle / 45f;

        // ==== LATERAL ERROR ====
        Vector3 toSpline = splinePos - transform.position;
        float lateralError = Vector3.Dot(toSpline, transform.right);
        float lateralSteer = lateralError * lateralCorrectionStrength;

        float finalSteer = Mathf.Clamp(
            (headingSteer + lateralSteer) * steeringSensitivity,
            -1f, 1f
        );

        // ==== BRAKING ====
        float curveAngle = Mathf.Abs(headingAngle);
        float speedFactor = Mathf.InverseLerp(maxCurveAngle, 0f, curveAngle);
        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedFactor);

        float motorInput = Mathf.Clamp(targetSpeed / maxSpeed, 0f, 1f);
        bool braking = rb.velocity.magnitude > targetSpeed;

        // APPLY
        wheels.SetSteer(finalSteer);
        wheels.SetMotor(braking ? 0f : motorInput);
        wheels.Brake(braking);
    }

    public void SetBlocked(bool value)
    {
        blocked = value;
    }

    public void SetSecondSplineActive(SplineContainer secondSpline)
    {
        spline = secondSpline;
        t = 0f;
        rb.velocity = Vector3.zero;
    }
}
