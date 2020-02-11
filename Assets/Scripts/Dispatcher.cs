using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dispatcher : MonoBehaviour
{
    public const int NumberOfPlayers = 8;
    private Transform[] catchers = new Transform[NumberOfPlayers];
    private Player[] players = new Player[NumberOfPlayers];
    private AIController[] a = new AIController[NumberOfPlayers];
    private Store[] stores = new Store[NumberOfPlayers];

    private int[] position = new int[NumberOfPlayers];
    private int[] opponent = new int[NumberOfPlayers];
    private int[] coinBuffer = new int[NumberOfPlayers];
    private List<int>[] dollBuffer = new List<int>[NumberOfPlayers];

    private string[] names = { "阿康", "巴纳纳", "你的克星", "欢笑的大表哥",
        "小狗狗", "天选之子", "酸菜余", "油茶小贩" };
    public int playerID;
    public string turnStage = "pick";
    
    private InputManager inputManager;
    private SpawnManager spawnManager;
    private StorageDisplayer storageDisplayer;
    private Transform mainCamera;

    private bool endGame = false;
    private float endCount = 10;

    void Start()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        storageDisplayer = GameObject.Find("Canvas").GetComponent<StorageDisplayer>();
        mainCamera = GameObject.Find("Main Camera").transform;
        Transform ai = GameObject.Find("AI").transform;
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            catchers[i] = GameObject.Find("Catcher (" + i + ")").transform;
            dollBuffer[i] = new List<int>();
            a[i] = ai.Find("AIController (" + i + ")").GetComponent<AIController>();
        }
        Reset();
    }

    void Reset()
    {
        Player.Reset();
        int ct = 0;
        int NumberOfNames = names.Length;
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            if (i % NumberOfNames == 0)
            {
                ct++;
            }
            string n = names[i % NumberOfNames];
            if (ct > 1)
            {
                n += ct.ToString();
            }
            players[i] = new Player(n);
            players[i].AddCoin(14); // purse
            stores[i] = new Store();
            a[i].id = i;
            a[i].store = stores[i];
            a[i].player = players[i];
        }
        int random = Random.Range(0, NumberOfPlayers);
        playerID = random;
        a[random].isAI = false;
        players[random].name = "我";
        storageDisplayer.player = players[random];
        storageDisplayer.store = stores[random];
        turnStage = "pick";
        Debug.Log("player ID:"+random);//todo
    }

    public void RefreshStore()
    {
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            stores[i].NewSlot();
        }
    }
    
    public void InitOpponent()
    {
        for(int i = 0; i < NumberOfPlayers; i++) 
        {
            position[i] = i;
        }
        SetCatcher();
    }

    public void UpdateOpponent()
    {
        int temp = position[NumberOfPlayers - 1];
        for (int i = NumberOfPlayers - 1; i > 1; i--)
        {
            position[i] = position[i - 1];
        }
        position[1] = temp;
        SetCatcher();
    }
    
    public int GetOpponentCost(int id)
    {
        return players[opponent[id]].cost;
    }

    void SetCatcher()
    {
        for(int i = 0; i < NumberOfPlayers / 2; i++)
        {
            opponent[position[i]] = position[NumberOfPlayers - 1 - i];
            opponent[position[NumberOfPlayers - 1 - i]] = position[i];
            a[position[i]].catcher = catchers[position[NumberOfPlayers - 1 - i]];
            a[position[NumberOfPlayers - 1 - i]].catcher = catchers[position[i]];
        }
        mainCamera.SendMessage("StartZoom", opponent[playerID]);
        inputManager.SetCatcher(a[playerID].catcher);
    }

    public bool CheckCoin(int id)
    {
        if (players[id].GetCoin() >= players[opponent[id]].cost)
        {
            return true;
        }
        return false;
    }

    public void PayCoin(int id)
    {
        players[id].RemoveCoin(players[opponent[id]].cost);
        coinBuffer[opponent[id]] += players[opponent[id]].cost;
    }

    public void FlushCoinBuffer()
    {
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            players[i].AddCoin(coinBuffer[i]);
            coinBuffer[i] = 0;
            //每个回合的福利在这里给
            players[i].AddCoin(3);
        }
    }

    public void GetDoll(int pos, int type)
    {
        players[pos].RemoveItem(type);
        dollBuffer[opponent[pos]].Add(type);
    }

    public void FlushDollBuffer()
    {
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            foreach(var type in dollBuffer[i])
            {
                players[i].AddItem(type);
                spawnManager.AddTask(i, type);
            }
            dollBuffer[i].Clear();
        }
    }

    public bool PurchaseDoll(int id, int slot)
    {
        int type = stores[id].GetItem(slot);
        int price = players[id].price[slot];
        if (type == 0 && players[id].save0)
        {
            price -= 1;
            if (price < 0)
            {
                price = 0;
            }
        }
        if (players[id].GetCoin() >= price && type != -1)
        {
            stores[id].BuyItem(slot);
            players[id].AddItem(type);
            players[id].RemoveCoin(price);
            spawnManager.AddTask(id, type);
            return true;
        }
        return false;
    }

    public void PlayerBuyDoll(int slot)
    {
        int type = stores[playerID].GetItem(slot);
        if (type == -1)
        {
            return;
        }
        //减费
        int price = players[playerID].price[slot];
        if (type == 0 && players[playerID].save0)
        {
            price -= 1;
            if (price < 0)
                price = 0;
        }
        if (players[playerID].GetCoin() < price)
        {
            return;
        }
        if (turnStage == "brawl")
        {
            stores[playerID].BuyItem(slot);
            dollBuffer[playerID].Add(type);
            players[playerID].RemoveCoin(price);
        }
        else if(turnStage == "pick")
        {
            stores[playerID].BuyItem(slot);
            players[playerID].AddItem(type);
            players[playerID].RemoveCoin(price);
            spawnManager.AddTask(playerID, type);
        }
        else
        {
            Debug.Log("Dispatcher.PlayerBuyDoll(): invalid turn stage");
        }
    }

    public void CheckWinner()
    {
        int maxIndex = -1;
        int maxCost = -1;
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            if (players[i].cost >= maxCost)
            {
                maxCost = players[i].cost;
                maxIndex = i;
            }
        }
        if (maxCost >= 15)
        {
            bool isWin = maxCost == players[playerID].cost;
            ShowRecord(isWin);
        }
    }

    private void ShowRecord(bool isWin)
    {
        string record = "";
        string score = "";
        int[] finalCost = new int[NumberOfPlayers];
        int[] pid = new int[NumberOfPlayers];
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            finalCost[i] = players[i].cost;
            pid[i] = i;
        }
        for(int i = NumberOfPlayers-1; i >0;i--)
        {
            bool flag = true;
            for(int j = 0; j < i; j++)
            {
                if(finalCost[j] < finalCost[j + 1])
                {
                    flag = false;
                    int temp = finalCost[j];
                    finalCost[j] = finalCost[j + 1];
                    finalCost[j + 1] = temp;
                    int temp_id = pid[j];
                    pid[j] = pid[j + 1];
                    pid[j + 1] = pid[j];
                }
            }
            if (flag)
            {
                break;
            }
        }
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            record += (i + 1).ToString() + ". " + players[pid[i]].name + "\n";
            score+= finalCost[i] + "\n";
        }
        Transform winlose = GameObject.Find("Canvas").transform.Find("Win&Lose");
        Text recordText = winlose.Find("Record").GetComponent<Text>();
        Text message = winlose.Find("Message").GetComponent<Text>();
        Text scoreText = winlose.Find("Score").GetComponent<Text>();
        recordText.text = record;
        scoreText.text = score;
        if (isWin)
        {
            message.text = "你赢了！！！";
        }
        else
        {
            message.text = "下次努力~";
        }
        winlose.gameObject.SetActive(true);
        endGame = true;
    }

    void Update()
    {
        if (endGame)
        {
            endCount -= Time.deltaTime;
        }
        if (endCount < 0)
        {
            Application.Quit();
        }
    }
}
