using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayer : MonoBehaviour
{
    public Transform[] spawnPoints;
    public int playerJoined = 0;

    public void OnPlayerJoined(PlayerInput input)
    {
        if(playerJoined < this.GetComponent<PlayerInputManager>().maxPlayerCount)
        {
            input.gameObject.GetComponent<PlayerManager>().spawnPos = spawnPoints[playerJoined].position;
            playerJoined+= 1;
        }
    }
}
