using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixerGroup soundEffectMixer;
    public AudioMixerGroup ostMixer;
    public AudioMixerGroup announcerMixer;

    [System.Serializable]
    public class KeyValue
    {
        public string audioName;
        public AudioClip audio;
    }
    public List<KeyValue> yes = new List<KeyValue>();
    public Dictionary<string, AudioClip> allAudio = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance != null)Destroy(this.gameObject);
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        foreach(var ui in yes)
        {
            allAudio[ui.audioName] = ui.audio;
        }
        
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos, AudioMixerGroup whatMixer, bool isSFX, bool islooping)
    {
        //Create GameObject
        GameObject tempGO = new GameObject("TempAudio");
        //pos of GO
        tempGO.transform.position = pos;
        //Add an audiosource
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        //Get the audio mixer
        audioSource.outputAudioMixerGroup = whatMixer;
        audioSource.loop = islooping;
        if(isSFX)audioSource.PlayOneShot(audioSource.clip);
        else audioSource.Play();
        //Destroy at the lenght of the clip
        if(!audioSource.loop)Destroy(tempGO, clip.length);
        return audioSource;
    }
}
