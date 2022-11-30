using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerprefab;
    
    
    // Start is called before the first frame update
    void Start()
    {
        var playerconfigs = PlayerConfigManager.Instance.GetPlayerConfig().ToArray();
        for (int i = 0; i < playerconfigs.Length; i++)
        {
            var player = Instantiate(playerprefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            GameManager.instance.InitializePlayer(player.GetComponent<PlayerInput>(), playerconfigs[i].PlayerIndex);
        }
    }

    
}
