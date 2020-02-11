using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float delta = 0.12f;
    public bool freeze = true;
    public Transform catcher;
    public bool isAI = true;
    public int id;
    public Store store;
    public Player player;
    private Dispatcher dispatcher;
    private Transform catchTarget;
    private Transform claw;

    private Transform controller;
    private Transform stick;
    private Transform button;
    private ClawCatch clawCatch;

    private float prepare = 0;
    private bool prepareFlag = false;

    void Start()
    {
        dispatcher = GameObject.Find("Dispatcher").GetComponent<Dispatcher>();
    }
    
    void Update()
    {
        if (!isAI)
        {
            return;
        }

        if (prepareFlag)
        {
            prepare -= Time.deltaTime;
        }
        if (prepare < 0)
        {
            clawCatch.EnterCoins();
            dispatcher.PayCoin(id);
            freeze = false;
            prepareFlag = false;
            prepare = 0;
        }

        if (freeze)
        {
            return;
        }
        Moving();
    }

    private void Moving()
    {
        Vector3 vec = catchTarget.position - claw.position;
        vec.y = 0;
        if(Mathf.Abs(vec.x) <= delta && Mathf.Abs(vec.z) <= delta)
        {
            controller.SendMessage("Stretch");
            freeze = true;
        }
        if (Mathf.Abs(vec.x) <= delta)
            vec.x = 0;
        else if (vec.x > 0)
            vec.x = 1;
        else
            vec.x = -1;
        if (Mathf.Abs(vec.z) <= delta)
            vec.z = 0;
        else if (vec.z > 0)
            vec.z = 1;
        else
            vec.z = -1;
        Vector2 inputVec2 = new Vector2(vec.x, vec.z);
        controller.SendMessage("Move", inputVec2);
        stick.SendMessage("RotateStick", inputVec2);
    }
    
    public void PickStrategy()
    {
        if (!isAI)
        {
            return;
        }
        int a = Random.Range(0, 5);
        a = Mathf.Min(player.GetCoin() / 4, a);
        int[] used = new int[4];
        for(int i = 0; i < a; i++)
        {
            while (true)
            {
                int candidate = Random.Range(0, 4);
                if (used[candidate] == 0)
                {
                    used[candidate] = 1;
                    dispatcher.PurchaseDoll(id, candidate);
                    break;
                }
            }
        }
    }

    public void CatchStrategy()
    {
        if (!isAI)
        {
            return;
        }
        if (player.GetCoin() < dispatcher.GetOpponentCost(id))
        {
            return;
        }
        // only catch once
        GameObject[] dolls = GameObject.FindGameObjectsWithTag("Doll");
        List<GameObject> gList = new List<GameObject>();
        for(int i = 0; i < dolls.Length; i++)
        {
            if (Vector3.Distance(dolls[i].transform.position, catcher.position) <= 7)
            {
                gList.Add(dolls[i]);
            }
        }
        int n = gList.ToArray().Length;
        if (n == 0)
        {
            return;
        }
        n = Random.Range(0, n);
        catchTarget = gList.ToArray()[n].transform;
        controller = catcher.Find("Claw Controller");
        claw = controller.Find("Claw");
        button = catcher.Find("CatchButton");
        stick = catcher.Find("MoveStick");
        clawCatch = controller.GetComponent<ClawCatch>();

        prepareFlag = true;
        prepare = 2;
    }
}
