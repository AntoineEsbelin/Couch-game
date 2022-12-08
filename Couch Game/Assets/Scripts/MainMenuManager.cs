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
        optionMenu.SetActive(true);
        CloseMenu(MainMenu);
        StartCoroutine(SelectionMenu(firstObjectOption));
    }
    
    public void MapSelectionButton()
    {
        MapSelectionMenu.SetActive(true);
        CloseMenu(MainMenu);
        StartCoroutine(SelectionMenu(Map1Button));
    }
    
    public void BindingMenu()
    {
        bindingInputMenu.SetActive(true);
        CloseMenu(optionMenu);
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

    private IEnumerator SelectionMenu(GameObject SelectButton)
    {
        yield return new WaitForSeconds(0.1f);
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(SelectButton, new BaseEventData(eventSystem));
    }
}
