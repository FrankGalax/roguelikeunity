using System;
using System.Collections.Generic;

public class Signal
{
    private List<Action> m_Slots;

    public Signal()
    {
        m_Slots = new List<Action>();
    }

    public void SendSignal()
    {
        foreach (Action slot in m_Slots)
        {
            slot();
        }
    }

    public void AddSlot(Action action)
    {
        m_Slots.Add(action);
    }
}
