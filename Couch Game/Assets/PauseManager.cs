using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [HideInInspector]public static bool gameIsPaused;
    public InputAction PauseAction;
    public GameObject PauseUI;
    public GameObject ResumeButton;
    
    void Awake()
    {
        PauseAction.Enable();
        PauseAction.performed += ctx => PauseGame();
    }
    
    void OnDisable()
    {
        PauseAction.Disable();
    }
    
    public void PauseGame()
    {

        gameIsPaused = !gameIsPaused;
        if (gameIsPaused)
        {
            PauseUI.SetActive(true);
            StartCoroutine(SelectionMenu(ResumeButton));
            Time.timeScale = 0f;
            
        }
        else
        {
            Resume();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        PauseUI.SetActive(false);
    }

    public void LoadScene(string name)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }
    
    private IEnumerator SelectionMenu(GameObject SelectButton)
    {
        yield return new WaitForSeconds(0.1f);
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(SelectButton, new BaseEventData(eventSystem));
    }
}
