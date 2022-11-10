using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerState
{
    public float maxRespawnTime = 3f;
    public float respawnTime;
    public override void EnterState(PlayerController player)
    {
        player.rb.velocity = Vector3.zero;
        player.ResetCharging();
        player.GetComponent<Stretch>().enabled = false;
        player.cameraTarget.targets.Remove(player.transform);
        respawnTime = maxRespawnTime;
        //visuel off :
        player.playerFBX.SetActive(false);
    }
    public override void UpdateState(PlayerController player)
    {
        if(respawnTime > 0)respawnTime -= Time.deltaTime;
        else player.stateMachine.SwitchState(player.NormalState);
    }
    public override void ExitState(PlayerController player)
    {
        //visuel on :
        Debug.Log("VISUEL ON");
        player.cameraTarget.targets.Add(transform);
        player.playerFBX.SetActive(true);
        player.RespawnPlayer();
    }
}
