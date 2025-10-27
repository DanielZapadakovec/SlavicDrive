using Ezereal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelFrictionController : MonoBehaviour
{
    [Header("Ezereal References")]
    [SerializeField] CarController carController;

    WheelFrictionCurve fLWSidewaysFriction;
    WheelFrictionCurve fRWSidewaysFriction;
    WheelFrictionCurve rLWSidewaysFriction;
    WheelFrictionCurve rRWSidewaysFriction;

    WheelFrictionCurve fLWForwardFriction;
    WheelFrictionCurve fRWForwardFriction;
    WheelFrictionCurve rLWForwardFriction;
    WheelFrictionCurve rRWForwardFriction;

    void Start()
    {
        if (carController != null)
        {
            SetForwardFriction();
            SetSidewaysFriction();
        }

    }

    void SetForwardFriction()
    {
        fLWForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.frontLeftWheelCollider.forwardFriction.extremumSlip,
            extremumValue = carController.frontLeftWheelCollider.forwardFriction.extremumValue,
            asymptoteSlip = carController.frontLeftWheelCollider.forwardFriction.asymptoteSlip,
            asymptoteValue = carController.frontLeftWheelCollider.forwardFriction.asymptoteValue,
            stiffness = carController.frontLeftWheelCollider.forwardFriction.stiffness
        };

        fRWForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.frontRightWheelCollider.forwardFriction.extremumSlip,
            extremumValue = carController.frontRightWheelCollider.forwardFriction.extremumValue,
            asymptoteSlip = carController.frontRightWheelCollider.forwardFriction.asymptoteSlip,
            asymptoteValue = carController.frontRightWheelCollider.forwardFriction.asymptoteValue,
            stiffness = carController.frontRightWheelCollider.forwardFriction.stiffness
        };

        rLWForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.rearLeftWheelCollider.forwardFriction.extremumSlip,
            extremumValue = carController.rearLeftWheelCollider.forwardFriction.extremumValue,
            asymptoteSlip = carController.rearLeftWheelCollider.forwardFriction.asymptoteSlip,
            asymptoteValue = carController.rearLeftWheelCollider.forwardFriction.asymptoteValue,
            stiffness = carController.rearLeftWheelCollider.forwardFriction.stiffness
        };

        rRWForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.rearRightWheelCollider.forwardFriction.extremumSlip,
            extremumValue = carController.rearRightWheelCollider.forwardFriction.extremumValue,
            asymptoteSlip = carController.rearRightWheelCollider.forwardFriction.asymptoteSlip,
            asymptoteValue = carController.rearRightWheelCollider.forwardFriction.asymptoteValue,
            stiffness = carController.rearRightWheelCollider.forwardFriction.stiffness
        };
    }

    void SetSidewaysFriction()
    {
        fLWSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.frontLeftWheelCollider.sidewaysFriction.extremumSlip,
            extremumValue = carController.frontLeftWheelCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = carController.frontLeftWheelCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = carController.frontLeftWheelCollider.sidewaysFriction.asymptoteValue,
            stiffness = carController.frontLeftWheelCollider.sidewaysFriction.stiffness
        };

        fRWSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.frontRightWheelCollider.sidewaysFriction.extremumSlip,
            extremumValue = carController.frontRightWheelCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = carController.frontRightWheelCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = carController.frontRightWheelCollider.sidewaysFriction.asymptoteValue,
            stiffness = carController.frontRightWheelCollider.sidewaysFriction.stiffness
        };

        rLWSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.rearLeftWheelCollider.sidewaysFriction.extremumSlip,
            extremumValue = carController.rearLeftWheelCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = carController.rearLeftWheelCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = carController.rearLeftWheelCollider.sidewaysFriction.asymptoteValue,
            stiffness = carController.rearLeftWheelCollider.sidewaysFriction.stiffness
        };

        rRWSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = carController.rearRightWheelCollider.sidewaysFriction.extremumSlip,
            extremumValue = carController.rearRightWheelCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = carController.rearRightWheelCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = carController.rearRightWheelCollider.sidewaysFriction.asymptoteValue,
            stiffness = carController.rearRightWheelCollider.sidewaysFriction.stiffness
        };
    }

    public void StartDrifting(float currentHandbrakeValue)
    {
        if (carController != null)
        {
            //use if you need to

            //rLwheelForwardFriction.extremumSlip = 
            //rRwheelForwardFriction.extremumSlip = 
            //rLwheelForwardFriction.extremumValue = 
            //rRwheelForwardFriction.extremumValue = 

            rLWSidewaysFriction.extremumSlip = 3f * currentHandbrakeValue;
            rRWSidewaysFriction.extremumSlip = 3f * currentHandbrakeValue;
            rLWSidewaysFriction.extremumValue = 0.7f * currentHandbrakeValue;
            rRWSidewaysFriction.extremumValue = 0.7f * currentHandbrakeValue;

            //Debug.Log(rLWSidewaysFriction.extremumSlip.ToString());

            //ezerealCarController.rearLeftWheelCollider.forwardFriction = rLwheelForwardFriction;
            //ezerealCarController.rearRightWheelCollider.forwardFriction = rRwheelForwardFriction;

            carController.rearLeftWheelCollider.sidewaysFriction = rLWSidewaysFriction;
            carController.rearRightWheelCollider.sidewaysFriction = rRWSidewaysFriction;
        }
    }

    public void StopDrifting()
    {
        if (carController != null)
        {
            //use if you need to

            //rLwheelForwardFriction.extremumSlip = 
            //rRwheelForwardFriction.extremumSlip = 
            //rLwheelForwardFriction.extremumValue = 
            //rRwheelForwardFriction.extremumValue = 

            //Set default value here
            rLWSidewaysFriction.extremumSlip = 0.2f;
            rRWSidewaysFriction.extremumSlip = 0.2f;
            rLWSidewaysFriction.extremumValue = 1f;
            rRWSidewaysFriction.extremumValue = 1f;

            //Debug.Log(rLWSidewaysFriction.extremumSlip.ToString());

            //ezerealCarController.rearLeftWheelCollider.forwardFriction = rLwheelForwardFriction;
            //ezerealCarController.rearRightWheelCollider.forwardFriction = rRwheelForwardFriction;

            carController.rearLeftWheelCollider.sidewaysFriction = rLWSidewaysFriction;
            carController.rearRightWheelCollider.sidewaysFriction = rRWSidewaysFriction;
        }
    }
}
