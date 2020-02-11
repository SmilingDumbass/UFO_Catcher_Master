using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickMove : MonoBehaviour
{
    public float rotateSpeed;
    public float maxAngle;
    public float returnSpeed;

    private Transform stick;
    private Transform anchor;
    private float xAccumulation = 0;
    private float zAccumulation = 0;

    void Start()
    {
        stick = transform.Find("Stick");
        anchor = transform.Find("Anchor");
    }
    
    void RotateStick(Vector2 vector)
    {
        // We never lock the stick!
        float xAngle = vector.x * rotateSpeed;
        float zAngle = -vector.y * rotateSpeed;

        // Return to idle position
        if (xAngle == 0)
        {
            xAngle = (xAccumulation / maxAngle) * (xAccumulation / maxAngle);
            xAngle *= xAccumulation > 0 ? -returnSpeed : returnSpeed;
        }
        if (zAngle == 0)
        {
            zAngle = (zAccumulation / maxAngle) * (zAccumulation / maxAngle);
            zAngle *= zAccumulation > 0 ? -returnSpeed : returnSpeed;
        }

        // Multiply delta time
        xAngle *= Time.deltaTime;
        zAngle *= Time.deltaTime;

        // Check bound
        if (xAngle + xAccumulation > maxAngle || xAngle + xAccumulation < -maxAngle)
        {
            xAngle = 0;
        }
        if (zAngle + zAccumulation > maxAngle || zAngle + zAccumulation < -maxAngle)
        {
            zAngle = 0;
        }

        // Rotate the stick and save the status
        stick.RotateAround(anchor.position, anchor.forward, xAngle);
        stick.RotateAround(anchor.position, anchor.right, zAngle);
        xAccumulation += xAngle;
        zAccumulation += zAngle;
    }
}
