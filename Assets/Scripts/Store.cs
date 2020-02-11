using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store
{
    private const int slotNumber = 4;
    private int[] slot = new int[slotNumber];

    public Store()
    {
        for(int i = 0; i < slot.Length; i++)
        {
            slot[i] = -1;
        }
    }
    
    public void NewSlot()
    {
        for(int i = 0; i < slot.Length; i++)
        {
            slot[i] = Random.Range(0, Player.total);
        }
    }

    public bool BuyItem(int slotIndex)
    {
        if (slot[slotIndex] != -1)
        {
            slot[slotIndex] = -1;
            return true;
        }
        return false;
    }

    public int GetItem(int slotIndex)
    {
        return slot[slotIndex];
    }
}
