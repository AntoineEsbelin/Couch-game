using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject selectPlayerMenu;
    public GameObject optionMenu;
    public GameObject mapSelectMenu;
    

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
        mapSelectMenu.SetActive(true);
    }

    private void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
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
        
        if (mapSelectMenu.activeInHierarchy)
        {
            if(cancel.WasPressedThisFrame())
                CloseMenu(mapSelectMenu);
        }
    }
}
