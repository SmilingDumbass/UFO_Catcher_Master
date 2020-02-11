using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchButton : MonoBehaviour
{
    public float depth;
    private Transform button;
    private Vector3 idlePosition;
    private bool pressed = false;

    void Start()
    {
        button = transform.Find("Button");
        idlePosition = button.position;
    }

    void CatchButtonDown()
    {
        if (!pressed)
        {
            pressed = true;
            button.Translate(new Vector3(0, 0, -depth), Space.Self);
        }
    }

    void CatchButtonUp()
    {
        button.position = idlePosition;
        pressed = false;
    }
}
