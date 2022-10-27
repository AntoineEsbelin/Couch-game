using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnLocations;
    
    public static PlayerSpawnManager instance;
    public bool isController;
    [HideInInspector]public PlayerInput _playerInput;

    private void Awake()
    {
        if(instance != null)Destroy(gameObject);
        instance = this;
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        if (isController)
        {
            // Set the player ID, add one to the index to start at Player 1
            playerInput.gameObject.GetComponent<ToupieBehaviour>().playerID = playerInput.playerIndex + 1;
            
            // Set the start spawn position of the player using the location at the associated element into the array.
            playerInput.gameObject.GetComponent<ToupieBehaviour>().startPos = spawnLocations[playerInput.playerIndex].position;
        }
        else
        {
            playerInput.gameObject.GetComponent<PlayerController>().input.playerID = playerInput.playerIndex + 1;
            playerInput.gameObject.GetComponent<PlayerController>().input.startPos = spawnLocations[playerInput.playerIndex].position;
        }
    }
}
