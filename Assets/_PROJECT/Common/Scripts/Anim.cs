using System.Collections.Generic;
using UnityEngine;

class AnimationPool : List<Anim>
{
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
    private Callback onFinished;

    public Anim(float startValue, float speed, float? endValue = null)
    {
        this.speed = speed;
        value = startValue;

        if (endValue != null)
            animate(endValue.Value);
    }

    public Anim animate(float endValue)
    {
        start = value;
        end = endValue;
        alpha = 0;

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

    public void step()
    {
        if (changed = (alpha != 1))
        {
            alpha = Mathf.Min(1, alpha + speed);
            value = Mathf.SmoothStep(start, end, alpha);

            if (onFinished != null && alpha == 1)
            {
                Callback tmp = onFinished;
                onFinished = null;
                tmp();
            }
        }
    }
}