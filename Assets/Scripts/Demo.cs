using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public float rotateSpeed;

    private Vector3 anchor;

    private Transform button;
    private Transform stick;
    private Transform controller;

    private Camera mainCamera;
    private Camera camera2;

    void Start()
    {
        Transform catcher = GameObject.Find("Catcher").transform;
        anchor = catcher.position;
        button = catcher.Find("CatchButton");
        stick = catcher.Find("MoveStick");
        controller = catcher.Find("Claw Controller");
        mainCamera = transform.GetComponent<Camera>();
        camera2 = controller.Find("Claw").Find("Camera").GetComponent<Camera>();
        camera2.enabled = false;
    }
    
    void Update()
    {
        // Move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector2 inputVec2 = new Vector2(x, z);
        stick.SendMessage("RotateStick", inputVec2);
        controller.SendMessage("Move", inputVec2);

        // Coin
        if (Input.GetKeyDown("c"))
        {
            controller.SendMessage("EnterCoins");
        }

        // Catch
        if (Input.GetKeyDown("space"))
        {
            button.SendMessage("CatchButtonDown");
            controller.SendMessage("Stretch");
        }
        if (Input.GetKeyUp("space"))
        {
            button.SendMessage("CatchButtonUp");
        }

        // Camera
        if (Input.GetKey("q") && Input.GetKey("e"))
        {
            return;
        }
        if (Input.GetKey("q"))
        {
            transform.RotateAround(anchor, Vector3.up, rotateSpeed * Time.deltaTime);
        }
        else if (Input.GetKey("e"))
        {
            transform.RotateAround(anchor, Vector3.up, -rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown("r"))
        {
            mainCamera.enabled = !mainCamera.enabled;
            camera2.enabled = !camera2.enabled;
        }
    }
}
