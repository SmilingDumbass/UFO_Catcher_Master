using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private Dispatcher dispatcher;
    private int id;

    void Start()
    {
        dispatcher = GameObject.Find("Dispatcher").GetComponent<Dispatcher>();
        id = int.Parse(transform.name.Substring(9, 1));
    }

    void OnTriggerEnter(Collider collider)
    {
        GameObject obj = collider.gameObject;
        if (obj.name.Substring(0, 7) == "RagDoll")
        {
            obj.layer = 10; // ignore collector
            int type = int.Parse(obj.name.Substring(7, 1));
            dispatcher.GetDoll(id, type);
            Destroy(obj, 5); //todo: replace this for mouse click
        }
    }
}
