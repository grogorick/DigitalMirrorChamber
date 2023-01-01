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

    public delegate void CallbackFloat(float value);
    public delegate void CallbackVoid();

    private CallbackFloat onStep;
    private CallbackVoid onFinished;
    private CallbackVoid onFinishedOnce;

    public Anim(float startValue, float speed, CallbackFloat onStep = null, CallbackVoid onFinished = null)
    {
        this.speed = speed;
        value = startValue;

        this.onStep = onStep;
        this.onFinished = onFinished;
    }

    public Anim animate(float endValue)
    {
        start = value;
        end = endValue;
        alpha = (start != end) ? 0 : (1 - speed * .5f);
        return this;
    }

    public Anim animateDiff(float diffValue, float clampMin = 0, float clampMax = 1)
    {
        return animate(Mathf.Clamp(value + diffValue, clampMin, clampMax));
    }
    
    public Anim then(CallbackVoid onFinishedOnceCallback)
    {
        onFinishedOnce = onFinishedOnceCallback;
        return this;
    }

    // To be called in any Update() or FixedUpdate() call, when not used with AnimationPool.
    public void step()
    {
        if (changed = (alpha != 1))
        {
            alpha = Mathf.Min(1, alpha + speed);
            value = Mathf.SmoothStep(start, end, alpha);

            if (onStep != null)
                onStep(value);

            if (alpha == 1)
                finish();
        }
    }

    private void finish()
    {
        if (onFinishedOnce != null)
        {
            CallbackVoid tmp = onFinishedOnce;
            onFinishedOnce = null;
            tmp();
        }
        else
            onFinished?.Invoke();
    }
}