using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    
    public float maxRespawnTime = 3f;
    public float respawnTime;

    public GameObject normalPlayer;
    public PlayerManager playerManager;
    private void OnEnable()
    {
        respawnTime = maxRespawnTime;
        normalPlayer.SetActive(false);
        playerManager.enabled = false;
        playerManager.GetComponent<Stretch>().enabled = false;
    }

    private void OnDisable()
    {
        normalPlayer.SetActive(true);
        playerManager.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(respawnTime > 0)respawnTime -= Time.deltaTime;
        else this.enabled = false;
    }
}
