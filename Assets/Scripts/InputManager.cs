using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Transform catcher;
    private Transform button;
    private Transform stick;
    private Transform controller;
    private Transform mainCamera;
    private ClawCatch clawCatch;
    private Dispatcher dispatcher;
    public bool coinLock;
    public int id;


    public void SetCatcher(Transform c)
    {
        catcher = c;

        button = catcher.Find("CatchButton");
        stick = catcher.Find("MoveStick");
        controller = catcher.Find("Claw Controller");
        clawCatch = controller.GetComponent<ClawCatch>();
        id = dispatcher.playerID;
    }

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").transform;
        dispatcher = GameObject.Find("Dispatcher").GetComponent<Dispatcher>();

        button = catcher.Find("CatchButton");
        stick = catcher.Find("MoveStick");
        controller = catcher.Find("Claw Controller");
        clawCatch = controller.GetComponent<ClawCatch>();
        id = dispatcher.playerID;
    }
    
    void Update()
    {
        // Quit
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        // Move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector2 inputVec2 = new Vector2(x, z);
        stick.SendMessage("RotateStick", inputVec2);
        controller.SendMessage("Move", inputVec2);

        // Coin
        if ((!coinLock)&&Input.GetKeyDown("c"))
        {
            if (!dispatcher.CheckCoin(id))
                return;
            if (clawCatch.EnterCoins())
            {
                dispatcher.PayCoin(id);
            }
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

        // Camera Move
        if (Input.GetKey("q") && Input.GetKey("e"))
        {
        }
        else if (Input.GetKey("q"))
        {
            mainCamera.SendMessage("RotateCamera", 1);
        }
        else if (Input.GetKey("e"))
        {
            mainCamera.SendMessage("RotateCamera", -1);
        }
        
    }
}
