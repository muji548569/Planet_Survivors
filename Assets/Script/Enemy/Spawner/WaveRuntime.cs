using UnityEngine;

public class WaveRuntime
{
    public WaveData data;
    public float nextTriggerTime;

    public WaveRuntime(WaveData data)
    {
        this.data = data;
        nextTriggerTime = data.startTime;
    }
}
