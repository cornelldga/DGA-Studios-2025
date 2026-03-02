using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector] public AudioSource source;
    public string name;
    public AudioClip audioClip;
    public bool isLoop;

    public bool playOnAwake;

    [Tooltip("can be in a range from 0 -> 1, percentage of gain from original audio source")]
    public float volume = 1.0f;
}