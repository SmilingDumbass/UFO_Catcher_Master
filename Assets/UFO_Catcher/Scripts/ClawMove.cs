using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawMove : MonoBehaviour
{
    public float speed;
    public float edge;

    private Transform claw;
    private float xAccumulation = 0;
    private float zAccumulation = 0;
    private bool isLock = true;
    
    void Start()
    {
        claw = transform.Find("Claw");
    }

    void Move(Vector2 vector)
    {
        if (isLock)
        {
            return;
        }
        float xTranslation = vector.x * speed * Time.deltaTime;
        float zTranslation = vector.y * speed * Time.deltaTime;
        if (xTranslation + xAccumulation <= 0 || xTranslation + xAccumulation >= edge)
        {
            xTranslation = 0;
        }
        if (zTranslation + zAccumulation <= 0 || zTranslation + zAccumulation >= edge)
        {
            zTranslation = 0;
        }
        claw.Translate(new Vector3(-xTranslation, 0, -zTranslation), Space.Self);
        xAccumulation += xTranslation;
        zAccumulation += zTranslation;
    }

    void Lock()
    {
        isLock = true;
        xAccumulation = 0;
        zAccumulation = 0;
    }

    void Unlock()
    {
        isLock = false;
    }

    void OnDrawGizmos()
    {
        Vector3[] points = new Vector3[4];
        Vector3 uDirection = -transform.forward * edge;
        Vector3 vDirection = -transform.right * edge;
        points[0] = transform.position;
        points[1] = points[0] + uDirection;
        points[2] = points[0] + uDirection + vDirection;
        points[3] = points[0] + vDirection;
        Gizmos.color = Color.yellow;
        for(int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % 4]);
        }
    }
}
