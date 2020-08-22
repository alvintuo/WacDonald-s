using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
          //  DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;

            if (s.loop)
            {
                s.source.Play();
                s.source.Pause();
            }
        }
    }

    public void Play(string soundName)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == soundName)
            {
                if (!s.source.isPlaying)
                {
                    s.source.Play();
                }
                return;
            }
        }
        print("Sound to play not found");
    }

    public void Pause(string soundName)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == soundName)
            {
                s.source.Pause();
                return;
            }
        }
        print("Sound to pause not found");
    }

    public void UnPause(string soundName)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == soundName)
            {
                if (!s.source.isPlaying)
                    s.source.UnPause();
                return;
            }
        }
        print("Sound to unpause not found");
    }

    public void PauseAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Pause();
        }
    }

    public void PausePlaying()
    {
        foreach (Sound s in sounds)
        {
            s.wasPlaying = s.source.isPlaying;
            s.source.Pause();
        }
    }

    public void UnPausePlaying()
    {
        foreach (Sound s in sounds)
        {
            if (s.wasPlaying)
            {
                s.source.UnPause();
            }
            s.wasPlaying = false;
        }
    }

    public Sound GetSound(string soundName)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == soundName)
            {
                return s;
            }
        }
        print("Sound to get not found");
        return null;
    }
}

[System.Serializable]
public class Sound
{

    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;

    public bool wasPlaying = false;
}