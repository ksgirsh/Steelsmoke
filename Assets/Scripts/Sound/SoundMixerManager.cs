using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(level) * 20f);
    }

    public void SetSoundVolume(float level)
    {
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(level) * 20f);
    }

    public void SetAmbiVolume(float level)
    {
        audioMixer.SetFloat("Ambience Volume", Mathf.Log10(level) * 20f);
    }
}
