using System.Collections.Generic;
using UnityEngine;

class AnimationPool : List<Anim>
{
    // To be called in any Update() or FixedUpdate() call.
    public void update()
    {
        foreach (var anim in this)
            anim.step();
    }
}

class Anim
{
    public float speed;
    public bool changed { get; private set; } = false;
    public float value { get; private set; }

    private float alpha = 1, start = 0, end = 0;

    public delegate void Callback();
    private Callback onStep;
    private Callback onFinished;
    private Callback onFinishedOnce;

    public Anim(float startValue, float speed, Callback onStep = null, Callback onFinished = null)
    {
        this.speed = speed;
        value = startValue;

        this.onStep = onStep;
        this.onFinished = onFinished;
    }

    public Anim animate(float endValue)
    {
        if (value != endValue)
        {
            start = value;
            end = endValue;
            alpha = 0;
        }
        return this;
    }

    public Anim animateDiff(float diffValue, float clampMin = 0, float clampMax = 1)
    {
        return animate(Mathf.Clamp(value + diffValue, clampMin, clampMax));
    }
    
    public void then(Callback onFinishedCallback)
    {
        onFinished = onFinishedCallback;
    }
    
    public void thenOnce(Callback onFinishedOnceCallback)
    {
        onFinishedOnce = onFinishedOnceCallback;
    }

    // To be called in any Update() or FixedUpdate() call, when not used with AnimationPool.
    public void step()
    {
        if (changed = (alpha != 1))
        {
            alpha = Mathf.Min(1, alpha + speed);
            value = Mathf.SmoothStep(start, end, alpha);

            if (onStep != null)
                onStep();

            if (alpha == 1)
            {
                if (onFinishedOnce != null)
                {
                    Callback tmp = onFinishedOnce;
                    onFinishedOnce = null;
                    tmp();
                }
                else
                {
                    onFinished?.Invoke();
                }
            }
        }
    }
}