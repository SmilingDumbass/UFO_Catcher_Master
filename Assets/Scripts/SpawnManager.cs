using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float interval;
    public GameObject[] dolls;
    private Transform[] spawnPoints = new Transform[Dispatcher.NumberOfPlayers];
    private float timeCount = 0;
    private bool[] flag = new bool[Dispatcher.NumberOfPlayers];
    private ArrayList[] tasks = new ArrayList[Dispatcher.NumberOfPlayers];

    void Start()
    {
        Transform sp = GameObject.Find("SP").transform;
        for (int i = 0; i < Dispatcher.NumberOfPlayers; i++)
        {
            spawnPoints[i] = sp.Find("SpawnPoint (" + i + ")");
            flag[i] = true;
            tasks[i] = new ArrayList();
        }
    }
    
    void Update()
    {
        timeCount += Time.deltaTime;
        if (timeCount > interval)
        {
            timeCount = 0;
            for(int i = 0; i < Dispatcher.NumberOfPlayers; i++)
            {
                flag[i] = true;
            }
        }
        for(int i = 0; i < Dispatcher.NumberOfPlayers; i++)
        {
            if (tasks[i].Count > 0)
            {
                if (flag[i])
                {
                    flag[i] = false;
                    int type = (int)tasks[i].ToArray()[0];
                    tasks[i].RemoveAt(0);
                    GameObject.Instantiate(dolls[type],
                        spawnPoints[i].position, spawnPoints[i].rotation);
                }
            }
        }
    }

    public void AddTask(int index, int type)
    {
        tasks[index].Add(type);
    }

    public void AddTask(Vector2Int vector)
    {
        AddTask(vector.x, vector.y);
    }
}
