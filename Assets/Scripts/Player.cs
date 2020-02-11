using UnityEngine;
using System.Collections;
public class Player
{
    public int id;
    public string name;
    private static int nextID = 0;
    public const int total = 8;
    private int[] collection = new int[total]; //player's doll collection
    public int cost = 2;
    private int coin = 0;
    private bool gift = false;
    public bool save0 = false;
    public int[] price= { 2, 2, 2, 2 };//todo

    public Player()
    {
        id = nextID++;
    }
    public Player(string n)
    {
        id = nextID++;
        name = n;
    }
    static public void Reset()
    {
        nextID = 0;
    }

    public bool AddItem(int index)
    {
        if (index >= total)
        {
            Debug.Log("AddItem: index out of bound");
            return false;
        }
        collection[index]++;
        RecalculateCost();
        return true;
    }

    public bool AddItem(int index, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if (!AddItem(index))
            {
                return false;
            }
        }
        return true;
    }

    public bool RemoveItem(int index)
    {
        if (index >= total)
        {
            Debug.Log("RemoveItem: index out of bound");
            return false;
        }
        if (collection[index] == 0)
        {
            Debug.Log("RemoveItem: not enough item");
            return false;
        }
        collection[index]--;
        RecalculateCost();
        return true; 
    }

    public bool RemoveItem(int index, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if (!RemoveItem(index))
            {
                return false;
            }
        }
        return true;
    }

    private void RecalculateCost()
    {
        int buff = 0;
        int amount = 0;
        for(int i = 0; i < total; i++)
        {
            amount += collection[i];
        }
        if ((collection[0] & collection[1] & collection[2] & collection[3] & collection[4]) != 0)
        {
            buff += 1;
        }
        if ((collection[5] & collection[6] & collection[7]) != 0)
        {
            buff += 1;
        }
        if (collection[0] >= 3)
        {
            save0 = true;
            if (collection[0] >= 5)
            {
                buff += 5;
            }
            else
            {
                buff += 1;
            }
        }
        if (collection[1] >= 3)
        {
            buff += 1;
            price[1] -= 1;
        }
        if (collection[2] >= 3)
        {
            buff += 2;
        }
        if (collection[3] >= 3)
        {
            if (amount >= 20)
            {
                buff += 3;
            }
            else
            {
                buff += 1;
            }
        }
        if (collection[4] >= 3)
        {
            buff += 3;
            for(int i = 0; i < price.Length; i++)
            {
                price[i] += 1;
            }
        }
        if (collection[6] >= 3)
        {
            buff += collection[6] - 2;
        }
        if (collection[7] >= 3)
        {
            buff += 1;
            if (!gift)
            {
                gift = true;
                Vector2Int vec = new Vector2Int(id, Random.Range(0, total));
                GameObject.Find("SpawnManager").SendMessage("AddTask", vec);
                AddItem(vec.y);
            }
        }
        if ((collection[0] & collection[2] & collection[4] & collection[6]) != 0)
        {
            if (amount % 2 == 0)
            {
                buff += 2;
            }
        }
        if ((collection[1] & collection[3] & collection[5] & collection[7]) != 0)
        {
            buff += 1;
        }
        if (collection[5] >= 3)
        {
            buff *= 2;
        }
        cost = 2 + buff;
    }

    public int EstimateCost(int[] types)
    {
        int[] tempCollection = new int[total];
        collection.CopyTo(tempCollection, 0);
        for(int i = 0; i < types.Length; i++)
        {
            tempCollection[types[i]]++;
        }
        return 0;//todo
    }

    public int GetCoin()
    {
        return coin;
    }

    public bool AddCoin(int amount)
    {
        coin += amount;
        return true;
    }

    public bool RemoveCoin(int amount)
    {
        if (coin < amount)
        {
            Debug.Log("RemoveCoin: not enough coin");
            return false;
        }
        coin -= amount;
        return true;
    }

    public int GetItem(int index)
    {
        return collection[index];
    }

    public string GetCollection()
    {
        string ret = "";
        for (int i = 0; i < total/2; i ++)
        {
            ret += i.ToString() + ": " + collection[i].ToString() + "  ";
            ret += (i + total / 2).ToString() + ": " + collection[i + total / 2].ToString() + "\n";
        }
        return ret;
    }
}
