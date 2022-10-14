using UnityEngine;

public class Stopwatch
{
    private float timer = 0.0f;

    private bool paused = false;

    public float GetElapsed { get { return timer; } }

    public Stopwatch()
    {
        this.timer = 0;
    }

    public void Update(float? timeStep = null)
    {
        if (paused) return;

        timer += (timeStep != null) ? (float)timeStep : Time.deltaTime;
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
        timer = 0;
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
