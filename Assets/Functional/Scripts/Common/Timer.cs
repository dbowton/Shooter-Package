using System;
using UnityEngine;

public class Timer
{
    private float timer = 0.0f;
    private float time = 0.0f;

    private bool paused = false;
    private bool autoReset = false;

    public bool IsOver { get { return timer <= 0; } }
    public float GetElapsed { get { if (time <= 0) return -1; return 1 - (timer / time); } }
    
    private Action function;

    public Timer()
    {
        this.time = 0;
        this.timer = time;
    }

    public Timer(Action function)
    {
        this.time = 0;
        this.timer = time;
        this.function = function;
    }

    public Timer(float time, Action function = null, bool autoReset = false)
    {
        this.time = time;
        this.timer = time;
        this.function = function;

        this.autoReset = autoReset;

        if (time <= 0) function?.Invoke(); 
    }

    public void Update(float? timeStep = null)
    {
        if (paused) return;

        if(timer > 0)
        {
            timer -= (timeStep != null) ? (float)timeStep : Time.deltaTime;

            if(timer <= 0)
            {
                function?.Invoke();
                if (autoReset) timer = time;
            }
        }
    }

    public void End()
    {
        timer = 0;
    }

    public void Modify(float time)
    {
        Modify(time, function);
    }
    public void Modify(float time, Action function)
    {
        Modify(time, function, autoReset);
    }
    public void Modify(float time, Action function, bool autoReset)
    {
        this.time = time;
        this.function = function;
        this.autoReset = autoReset;
    }

    public void Manipulate(float change)
    {
        timer += change;
    }

    public void Set(float setTime)
    {
        timer = setTime;
    }

    public void Reset()
    {
        timer = time;
    }

    public void Pause()
    {
        paused = true;
    }

    public void Resume()
    {
        paused = false;
    }
}
