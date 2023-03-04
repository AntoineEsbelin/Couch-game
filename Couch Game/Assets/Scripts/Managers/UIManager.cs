using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject[] playerUIPanels;

    private void Start()
    {
        GameManager.instance.PlayerJoinedGame += PlayerJoinedGame;
        GameManager.instance.PlayerLeftGame += PlayerLeftGame;
    }
    
    private void PlayerJoinedGame(PlayerInput playerInput)
    {
        ShowUIPanel(playerInput);
    }
    
    private void PlayerLeftGame(PlayerInput playerInput)
    {
        HideUIPanel(playerInput);
    }
    private void ShowUIPanel(PlayerInput playerInput)
    {
        playerUIPanels[playerInput.playerIndex].SetActive(true);
        playerUIPanels[playerInput.playerIndex].GetComponent<PlayerUIPanel>().AssignPlayer(playerInput.playerIndex);
    }
    
    private void HideUIPanel(PlayerInput playerInput)
    {
        playerUIPanels[playerInput.playerIndex].SetActive(false);
    }

    public void SelectSFX()
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Navigate"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }
}
