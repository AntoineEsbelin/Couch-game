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
    public GameObject selectPlayerMenu;
    public GameObject optionMenu;
    public GameObject MapSelectionMenu;
    
    public static MainMenuManager Instance { get; private set;}

    private void Awake()
    {
        if (Instance != null)
        {
            print("SINGLETON - Trying to create another instance");
        }
        else
        {
            Instance = this;
        }
    }


    public void StartButton()
    {
        selectPlayerMenu.SetActive(true);
    }
    
    public void OptionButton()
    {
        optionMenu.SetActive(true);
    }
    
    public void MapSelectButton()
    {
        MapSelectionMenu.SetActive(true);
    }

    private void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        var uiModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        InputAction cancel = uiModule.cancel.action;
        
        if (selectPlayerMenu.activeInHierarchy)
        {
            if(cancel.WasPressedThisFrame())
                CloseMenu(selectPlayerMenu);
        }
        
        if (optionMenu.activeInHierarchy)
        {
            if(cancel.WasPressedThisFrame())
                CloseMenu(optionMenu);
        }
        
        if (MapSelectionMenu.activeInHierarchy)
        {
            if(cancel.WasPressedThisFrame())
                CloseMenu(MapSelectionMenu);
        }
    }
}
