using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private Transform ClawController;

    void Start()
    {
        ClawController = transform.parent.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        ClawController.SendMessage("OnDetectorHit");
    }
}
