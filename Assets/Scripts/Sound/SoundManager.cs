using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume = 0.5f;
    [Range(0f, 10f)]
    public float pitch = 1f;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        source.volume = volume;
        source.pitch = pitch;
        source.loop = true;
        source.Play();
    }
    public void Stop()
    {
        source.Stop();
    }
    public void IncreaseVolume()
    {
        Debug.Log($"SoundManager: last sound {source.name} volume: {source.volume}");
        if (source.volume + 0.1f <= 1)
            source.volume += 0.1f;
        else
            source.volume = 1f;
        Debug.Log($"SoundManager: new sound {source.name} volume: {source.volume}");
    }
    public void DecreaseVolume()
    {
        Debug.Log($"SoundManager: last sound {source.name} volume: {source.volume}");
        if (source.volume - 0.1f >= 0)
            source.volume -= 0.1f;
        else
            source.volume = 0;
        Debug.Log($"SoundManager: new sound {source.name} volume: {source.volume}");
    }

    public void IncreasePitch()
    {
        Debug.Log($"SoundManager: last sound {source.name} pitch: {source.volume}");
        if (source.pitch + 0.1f <= 2)
            source.pitch += 0.1f;
        else
            source.pitch = 2f;
        Debug.Log($"SoundManager: new sound {source.name} pitch: {source.volume}");
    }
    public void DecreasePitch()
    {
        Debug.Log($"SoundManager: last sound {source.name} pitch: {source.volume}");
        if (source.pitch - 0.1f >= 0)
            source.pitch -= 0.1f;
        else
            source.pitch = 0;
        Debug.Log($"SoundManager: new sound {source.name} pitch: {source.volume}");
    }



    public void ChangeVolume(float newVolume)
    {
        source.volume = newVolume;
    }
    public void ChangePitch(float newPitch)
    {
        source.pitch = newPitch;
    }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instace;

    [SerializeField]
    Sound[] sounds;

    void Awake()
    {
        if (instace != null)
        {
            Debug.LogError("More than one SoundManger in the scene");
        }
        else
        {
            instace = this;
        }
    }

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        
    }
    public void PlaySound(string _name)
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        //no sound with _name
        Debug.Log($"SoundManager: Sound not found in list {_name}");
    }
    public void StopSounds()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].Stop();
        }
    }
    public float GetSoundVolume(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i].volume;
            }
        }
        Debug.Log($"SoundManager: Sound not found in list {_name}");
        return -1f;
    }
    public float GetSoundPitch(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i].pitch;
            }
        }
        Debug.Log($"SoundManager: Sound not found in list {_name}");
        return -1f;
    }
    public void IncreaseVolume(string _name)
    {
        Debug.Log($"SoundManager: Change Volume {_name}");
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].IncreaseVolume();
                return;
            }
        }
        //no sound with _name
        Debug.Log($"SoundManager: Sound not found in list {_name}");
    }

    public void DecreaseVolume(string _name)
    {
        Debug.Log($"SoundManager: Change Volume {_name}");
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].DecreaseVolume();
                return;
            }
        }
        //no sound with _name
        Debug.Log($"SoundManager: Sound not found in list {_name}");
    }
    public void IncreasePitch(string _name)
    {
        Debug.Log($"SoundManager: Increase Pitch {_name}");
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].IncreasePitch();
                return;
            }
        }
        //no sound with _name
        Debug.Log($"SoundManager: Sound not found in list {_name}");
    }
    public void DecreasePitch(string _name)
    {
        Debug.Log($"SoundManager: Decrease Pitch {_name}");
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].DecreasePitch();
                return;
            }
        }
        //no sound with _name
        Debug.Log($"SoundManager: Sound not found in list {_name}");
    }
}
