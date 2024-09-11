using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State
{
    public DataToStore rightHand;
    public DataToStore leftHand;
    public int note;

    public State(DataToStore l, DataToStore r, int n)
    {
        this.leftHand = l;
        this.rightHand = r;
        this.note = n;
    }

    public void set_note(int n)
    {
        this.note = n;
    }
}

