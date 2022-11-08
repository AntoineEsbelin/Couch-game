using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixerGroup soundEffectMixer;

    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        //Create GameObject
        GameObject tempGO = new GameObject("TempAudio");
        //pos of GO
        tempGO.transform.position = pos;
        //Add an audiosource
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        //Get the audio mixer
        audioSource.outputAudioMixerGroup = soundEffectMixer;
        audioSource.Play();
        //Destroy at the lenght of the clip
        Destroy(tempGO, clip.length);
        return audioSource;
    }
}
