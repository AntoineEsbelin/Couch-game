using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;

   AudioSource sfx;
    AudioSource ost;

    
    public void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution{width = resolution.width, height = resolution.height}).Distinct().ToArray();
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }   
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();

        Screen.fullScreen = true;

    }
    public void SetFullScreen(bool isFullScreen)
    {
        if(isFullScreen)SelectSFX();
        else BackSFX();
        
        Screen.fullScreen = isFullScreen;
        
    }

    public void SetSuicide(Toggle toggle)
    {
        if(toggle.isOn)SelectSFX();
        else BackSFX();
        PointZone.isSettingSuicideOn = toggle.isOn;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SelectSFX();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        TestOst();
    }
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        audioMixer.SetFloat("Announcer", volume);
        TestSFXAnnouncer();
    }

    private void SelectSFX()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Validate"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }

    private void TestOst()
    {
        if(ost != null)Destroy(ost.gameObject);
        AudioSource ostTest = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Validate"], this.transform.position, AudioManager.instance.ostMixer, true, false);
        ost = ostTest;
    }

    private void TestSFXAnnouncer()
    {
        if(sfx != null)Destroy(sfx.gameObject);
        AudioSource sfxTest = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Title"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        sfx = sfxTest;
    }

    private void BackSFX()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Back"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }
}
