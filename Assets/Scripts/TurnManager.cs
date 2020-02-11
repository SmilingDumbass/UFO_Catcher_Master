using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public float pickTime;
    public float brawlTime;
    public float warningTime;
    private int turnCount = -1;
    private float timeCount = 0;
    private int flag = 0;
    private Dispatcher dispatcher;
    private Transform mainCamera;
    private InputManager inputManager;
    private AIController[] a = new AIController[Dispatcher.NumberOfPlayers];
    
    void Start()
    {
        dispatcher = GameObject.Find("Dispatcher").GetComponent<Dispatcher>();
        mainCamera = GameObject.Find("Main Camera").transform;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        Transform ai = GameObject.Find("AI").transform;
        for(int i = 0; i < Dispatcher.NumberOfPlayers; i++)
        {
            a[i] = ai.Find("AIController (" + i + ")").GetComponent<AIController>();
        }

        brawlTime += pickTime;
        //todo:delete this
        //db(16);
    }

    void db(int n)
    {
        int cishu = 1000;
        int act01234567 = 0;
        int bct222 = 0;
        int c567 = 0;
        for(int i = 0; i < cishu; i++)
        {
            int[] a = new int[8];
            for (int j = 0; j < n; j++)
            {
                int random = Random.Range(0, 8);
                a[random]++;
            }

            bool aflag = true;
            for(int j = 0; j < 8; j++)
            {
                if (a[j] <= 0)
                {
                    aflag = false;
                    break;
                }
            }
            if (aflag)
            {
                act01234567++;
            }

            //
            if (a[0] >= 4)
            {
                bct222++;
            }

            if (a[0] > 0 && a[6] > 0 && a[4] > 0 && a[2] > 0)
            {
                c567++;
            }
        }
        Debug.Log("01234567: " + act01234567 + "/" + cishu);
        Debug.Log("6666: " + bct222 + "/" + cishu);
        Debug.Log("even: " + c567 + "/" + cishu);
    }

    //todo: 每次重置时调用该reset和dispatcher的reset
    void Reset()
    {
        turnCount = 0;
        timeCount = 0;
        flag = 0;
        dispatcher.RefreshStore();
    }

    //todo: add timer and warning
    void Update()
    {
        if (turnCount == -1)
        {
            Reset();
            for (int i = 0; i < Dispatcher.NumberOfPlayers; i++)
            {
                a[i].PickStrategy();
            }
        }
        
        timeCount += Time.deltaTime;

        //brawlBegin
        if (flag == 0 && timeCount > pickTime)
        {
            dispatcher.turnStage = "brawl";
            flag = 1;
            if (turnCount == 0)
            {
                dispatcher.InitOpponent();
            }
            else
            {
                dispatcher.UpdateOpponent();
            }
            inputManager.coinLock = false;
            for(int i = 0; i < Dispatcher.NumberOfPlayers; i++)
            {
                a[i].CatchStrategy();
            }
        }

        //brawlEnd, next pick begin
        if (timeCount > brawlTime && flag == 1)
        {
            dispatcher.turnStage = "pick";
            flag = 0;
            timeCount = 0;
            turnCount++;
            mainCamera.SendMessage("StartZoom", 8);
            dispatcher.FlushCoinBuffer();
            dispatcher.FlushDollBuffer();

            dispatcher.CheckWinner();

            dispatcher.RefreshStore();
            for(int i = 0; i < Dispatcher.NumberOfPlayers; i++)
            {
                a[i].PickStrategy();
            }
        }

        else if (timeCount > brawlTime - warningTime && flag == 1)
        {
            inputManager.coinLock = true;
        }
        
    }
}
