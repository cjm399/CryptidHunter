using UnityEngine.Audio;
using System;
using UnityEngine;

// To access audio clips in other classes simply do: 
// AudioManager.instance?.Play("INSERT_CLIP_NAME);

// This will also be a singleton class

// *******NOTE******** 
// Make sure Pitch in the Sound class is set to 1 and not .1.
// Will cause sound to not play/play very quitely

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
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
    
    void Start()
    {
        // Creates a new game object for each sound under the Audio Manager
        foreach (Sound s in sounds)
        {
            GameObject gObj = new GameObject("Sound_" + s.name);
            gObj.transform.SetParent(this.transform);
            s.SetSource(gObj.AddComponent<AudioSource>());

            if(s.source.playOnAwake)
            {
                Play(s.name);
            }
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log(name + " audio cannot be found");
            return;
        }
        s.Play();
        Debug.Log("Played Sound_" + name);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log(name + " audio cannot be found");
            return;
        }
        s.Stop();
        Debug.Log("Stopped Sound_" + name);
    }
}
