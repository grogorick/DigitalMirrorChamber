using System;
using UnityEngine;


[Serializable]
public class Debounce
{
    [Tooltip("Milliseconds")]
    public double delay;
    private DateTime lastTime;

    public Debounce (double delay = 250)
    {
        this.delay = delay;
    }

    public bool check()
    {
        if (delay == 0)
            return true;
        DateTime now = DateTime.Now;
        if (((TimeSpan)(now - lastTime)).TotalMilliseconds < delay)
            return false;
        lastTime = now;
        return true;
    }

    public void reset()
    {
        lastTime.AddMilliseconds(-delay);
    }
}