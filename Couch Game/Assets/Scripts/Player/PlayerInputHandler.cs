using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public GameObject playerPrefab;
    private PlayerManager _playerManager;

    private void Awake()
    {
        if (playerPrefab != null)
        {
            _playerManager = GameObject.Instantiate(playerPrefab, SpawnPlayer.instance.spawnPoints[0].position,
                    transform.rotation).GetComponent<PlayerManager>();
        }
    }
}
