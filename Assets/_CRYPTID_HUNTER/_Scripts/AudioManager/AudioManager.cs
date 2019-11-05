using UnityEngine.Audio;
using System;
using UnityEngine;

// To access audio clips in other classes simply do: 
// FindObjectOfType<AudioManager>().Play("soundYouWantToPlay");

// This will also be a singleton class
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Creates a new game object for each sound under the Audio Manager
    void Start()
    {
        foreach (Sound s in sounds)
        {
            GameObject sound = new GameObject("Sound_" + s.name);
            sound.transform.SetParent(this.transform);
            s.source = sound.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.priority = s.priority;

            if(s.source.playOnAwake)
            {
                Play(s.name);
            }
        }
    }

    public void Play(string name)
    {
        AudioSource s = GameObject.Find("Sound_" + name).GetComponent<AudioSource>();
        if (s == null)
        {
            Debug.Log(name + " auido cannot be found");
            return;
        }
        s.Play();
        Debug.Log("Played Sound_" + name);
    }

    public void Stop(string name)
    {
        AudioSource s = GameObject.Find("Sound_" + name).GetComponent<AudioSource>();
        if (s == null)
        {
            Debug.Log(name + " auido cannot be found");
            return;
        }
        s.Stop();
        Debug.Log("Stopped Sound_" + name);
    }
}
