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

    public GameObject MainMenu;
    public GameObject StartButton;
    public GameObject optionMenu;
    [Header("Map Selection")]
    public GameObject MapSelectionMenu;
    public GameObject Map1Button;

    public void MainMenuButton()
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(StartButton, new BaseEventData(eventSystem));
        MainMenu.SetActive(true);
    }
    public void OptionButton()
    {
        optionMenu.SetActive(true);
        CloseMenu(MainMenu);
    }
    
    public void MapSelectionButton()
    {
        MapSelectionMenu.SetActive(true);
        CloseMenu(MainMenu);
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(Map1Button, new BaseEventData(eventSystem));
        
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
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
            }
                
        }
        
        if (MapSelectionMenu.activeInHierarchy)
        {
            if (cancel.WasPressedThisFrame())
            {
                MainMenuButton();
                CloseMenu(MapSelectionMenu);
            }
                
        }
    }
}
