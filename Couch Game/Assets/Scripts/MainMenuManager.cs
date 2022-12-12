using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [Header("Main Menu")]
    public GameObject MainMenu;
    public GameObject StartButton;
    
    [Header("Options")]
    public GameObject optionMenu;
    public GameObject firstObjectOption;
    
    [Header("Map Selection")]
    public GameObject MapSelectionMenu;
    public GameObject Map1Button;

    [Header("Binding Menu")]
    public GameObject bindingInputMenu;
    public GameObject firstBinding;
    

    public void MainMenuButton()
    {
        MainMenu.SetActive(true);
        StartCoroutine(SelectionMenu(StartButton));
    }
    public void OptionButton()
    {
        SelectSFX();
        optionMenu.SetActive(true);
        CloseMenu(MainMenu);
        StartCoroutine(SelectionMenu(firstObjectOption));
    }
    
    public void MapSelectionButton()
    {
        SelectSFX();
        MapSelectionMenu.SetActive(true);
        CloseMenu(MainMenu);
        StartCoroutine(SelectionMenu(Map1Button));
    }
    
    public void BindingMenu()
    {
        SelectSFX();
        bindingInputMenu.SetActive(true);
        CloseMenu(optionMenu);
    }

    public void LoadLevel(string name)
    {
        StartCoroutine(OnLoadLevel(name));
    }

    private void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void Quit()
    {
        StartCoroutine(OnQuit());
    }

    private void Start()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Title"], this.transform.position, AudioManager.instance.announcerMixer, true, false);
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Title OST"], this.transform.position, AudioManager.instance.ostMixer, false, true);
    }

    private void Update()
    {
        var uiModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        InputAction cancel = uiModule.cancel.action;
        
        
        if (optionMenu.activeInHierarchy)
        {
            if (cancel.WasPressedThisFrame())
            {
                MainMenuButton();
                CloseMenu(optionMenu);
                ExitSFX();
            }
                
        }
        
        if (MapSelectionMenu.activeInHierarchy)
        {
            if (cancel.WasPressedThisFrame())
            {
                MainMenuButton();
                CloseMenu(MapSelectionMenu);
                ExitSFX();
            }
                
        }
    }

    private IEnumerator SelectionMenu(GameObject SelectButton)
    {
        yield return new WaitForSeconds(0.1f);
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(SelectButton, new BaseEventData(eventSystem));
    }

    public void OnSelect()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Navigate"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }

    private void SelectSFX()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Validate"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }

    private void ExitSFX()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Back"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }

    private IEnumerator OnQuit()
    {
        AudioSource sfx = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Exit"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        yield return new WaitForSeconds(sfx.clip.length / 2);
        Application.Quit();
    }

    private IEnumerator OnLoadLevel(string name)
    {
        AudioSource sfx = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Menu Transition"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        yield return new WaitForSeconds(sfx.clip.length / 2);
        SceneManager.LoadScene(name);
    }
}
