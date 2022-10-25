using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayer : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] charactersFBX;
    public GameObject[] spinnersFBX;

    [SerializeField] private List<GameObject> characterFBXAlreadySpawned;
    public int playerJoined = 0;

    public void OnPlayerJoined(PlayerInput input)
    {   
        if(playerJoined < this.GetComponent<PlayerInputManager>().maxPlayerCount)
        {
            input.GetComponent<PlayerManager>().playerId = playerJoined;
            input.GetComponent<PlayerManager>().spawnPlayer = this;
            RandomSpawnFBX(input);
            //assigne le model 3D selon le joueur
            playerJoined+= 1;
        }
    }

    private void RandomSpawnFBX(PlayerInput input)
    {
        int randomCharacter = Random.Range(0, (charactersFBX.Length));
        for(int i = 0; i < characterFBXAlreadySpawned.Count; i++)
        {
            if(characterFBXAlreadySpawned[i] == charactersFBX[randomCharacter])
            {
                RandomSpawnFBX(input);
                return;
            }
        }
        input.GetComponent<PlayerManager>().normalFBX = charactersFBX[randomCharacter];
        input.GetComponent<PlayerManager>().spinnerFBX = spinnersFBX[randomCharacter];
        characterFBXAlreadySpawned.Add(charactersFBX[randomCharacter]);
    }

    private void FixedUpdate()
    {
        
    }
}
