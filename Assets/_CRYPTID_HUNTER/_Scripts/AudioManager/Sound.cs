using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector]
    public AudioSource source;

    public string name;

    public AudioClip clip;

    public bool loop;
    public bool playOnAwake;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0, 255)]
    public int priority = 128;

    public void SetSource(AudioSource s)
    {
        source = s;
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.priority = priority;
    }

    public void Play()
    {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}
