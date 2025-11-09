using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointEnabler : MonoBehaviour
{
    public float speedtoEnable = 10f;
    public HingeJoint leftDoor, rightDoor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > speedtoEnable)
        {
            JointLimits leftLimits = leftDoor.limits;
            JointLimits rightLimits = rightDoor.limits;

            leftLimits.min = 0;
            leftLimits.max = 70f;

            rightLimits.min = -70;
            rightLimits.max = 0f;

            leftDoor.limits = leftLimits;
            rightDoor.limits = rightLimits;
        }
    }


}