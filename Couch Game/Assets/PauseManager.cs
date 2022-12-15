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
    public GameObject transition;

    private AudioSource pauseOST;
    
    void Awake()
    {
        PauseAction.Enable();
        PauseAction.performed += ctx => PauseGame();
    }
    
    void OnDisable()
    {
        PauseAction.Disable();
    }

    private void Start()
    {
        gameIsPaused = false;
    }
    
    public void PauseGame()
    {
        if(!GameManager.instance.gameStarted)return;
        gameIsPaused = !gameIsPaused;
        if (gameIsPaused)
        {
            PauseUI.SetActive(true);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Appear"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            if(GameManager.instance.ost != null)GameManager.instance.ost.Pause();
            pauseOST = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Choice OST"], this.transform.position, AudioManager.instance.soundEffectMixer, false, true);
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
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Back"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        if(GameManager.instance.ost != null)GameManager.instance.ost.UnPause();
        if(pauseOST != null)Destroy(pauseOST.gameObject);
        gameIsPaused = false;
        PauseUI.SetActive(false);
    }

    public void LoadScene(string name)
    {
        Time.timeScale = 1f;
        transition.GetComponent<Animator>().SetTrigger("In");
        StartCoroutine(ChangeScene(name));
      //  SceneManager.LoadScene(name);
    }
    
    private IEnumerator SelectionMenu(GameObject SelectButton)
    {
        yield return new WaitForSeconds(0.1f);
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(SelectButton, new BaseEventData(eventSystem));
    }

    private IEnumerator ChangeScene(string name)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(name);
    }
}
