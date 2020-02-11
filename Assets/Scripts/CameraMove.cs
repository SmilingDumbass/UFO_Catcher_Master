using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rotateSpeed;
    public float zoomTime;

    private Vector3 anchor;
    private Transform[] cameraPoints;
    private Vector3[] catcherPosition;
    private bool isLock = true;
    private bool zoom = false;
    private float zoomCount = 0;
    private Quaternion fromRotation;
    private Vector3 fromPosition;
    private Transform to;
    private int target;
    private Vector3 resetTo;
    private bool reset = false;

    void Start()
    {
        Transform cps = GameObject.Find("CameraPoints").transform;
        cameraPoints = new Transform[9];
        catcherPosition = new Vector3[8];
        for(int i = 0; i < 8; i++)
        {
            cameraPoints[i] = cps.Find("CameraPoint (" + i + ")");
            catcherPosition[i] = GameObject.Find("Catcher (" + i + ")").transform.position;
        }
        cameraPoints[8] = cps.Find("HighestPoint");
        transform.position = cameraPoints[8].position;
        transform.rotation = cameraPoints[8].rotation;
    }
    
    void Update()
    {
        if (reset)
        {
            Reset();
            return;
        }
        if (zoom)
        {
            Zoom();
        }
    }

    void RotateCamera(float sign)
    {
        if (isLock)
        {
            return;
        }
        transform.RotateAround(anchor, Vector3.up, sign * rotateSpeed * Time.deltaTime);
    }

    void Zoom()
    {
        transform.rotation = Quaternion.Slerp(fromRotation, to.rotation, zoomCount / zoomTime);
        transform.position = Vector3.Lerp(fromPosition, to.position, zoomCount / zoomTime);
        if (zoomCount >= zoomTime)
        {
            zoom = false;
            zoomCount = 0;
            if (target < 8)
            {
                isLock = false;
            }
            return;
        }
        zoomCount += Time.deltaTime;
    }

    void StartZoom(int target)
    {
        if (this.target == target)
        {
            return;
        }
        if (target == 8)
        {
            resetTo = cameraPoints[this.target].position - anchor;
            resetTo.y = 0;
            reset = true;
        }
        else if (target < 8)
        {
            anchor = catcherPosition[target];
            fromPosition = transform.position;
            fromRotation = transform.rotation;
        }
        this.target = target;
        to = cameraPoints[target];
        isLock = true;
        zoom = true;
    }
    
    void Reset()
    {
        Vector3 resetFrom = transform.position - anchor;
        resetFrom.y = 0;
        if (Vector3.Angle(resetFrom, resetTo) < 3)
        {
            reset = false;
            fromPosition = transform.position;
            fromRotation = transform.rotation;
        }
        if (Vector3.Cross(resetFrom, resetTo).y >= 0)
        {
            transform.RotateAround(anchor, Vector3.up, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(anchor, Vector3.up, -rotateSpeed * Time.deltaTime);
        }
    }
    
}
