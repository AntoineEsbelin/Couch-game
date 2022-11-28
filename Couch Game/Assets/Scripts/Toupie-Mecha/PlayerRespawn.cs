using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    
    public float maxRespawnTime = 3f;
    public float respawnTime;

    public GameObject normalPlayer;
    public PlayerManager playerManager;
    public Rigidbody rb;
    private void OnEnable()
    {
        respawnTime = maxRespawnTime;
        normalPlayer.SetActive(false);
        playerManager.enabled = false;
        normalPlayer.GetComponent<NormalControler>().enabled = true;
    }

    private void OnDisable()
    {
        normalPlayer.SetActive(true);
        playerManager.enabled = true;
        //rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(respawnTime > 0)respawnTime -= Time.deltaTime;
        else this.enabled = false;
    }
}
