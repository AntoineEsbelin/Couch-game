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
        player.cameraTarget.targets.Remove(player.transform);
        respawnTime = maxRespawnTime;
        WallEvent wallEvent = GameObject.FindObjectOfType<WallEvent>();
        if(wallEvent != null)player.NeonBugBounce(wallEvent);
        //visuel off :
        ResetAnimator(player.PlayerAnimator);
        player.playerFBX.SetActive(false);
        if(player.hasDaCrown)player.playerCrown.SetActive(false);
        player.GetComponentInChildren<SpinningAnim>(true).transform.localScale = Vector3.one;
    }
    public override void UpdateState(PlayerController player)
    {
        if(respawnTime > 0)respawnTime -= Time.deltaTime;
        else player.stateMachine.SwitchState(player.NormalState);
    }
    public override void ExitState(PlayerController player)
    {
        //visuel on :
        player.cameraTarget.targets.Add(transform);
        player.PlayerAnimator.enabled = true;
        player.playerFBX.SetActive(true);
        if(player.hasDaCrown)player.playerCrown.SetActive(true);
        player.RespawnPlayer();
    }


    private void ResetAnimator(Animator playerAnim)
    {
        
        playerAnim.Play("Idle");
        playerAnim.SetBool("IsWalking", false);
        playerAnim.SetBool("IsSpinning", false);
        playerAnim.SetBool("Counter", false);
        playerAnim.SetBool("ChargingSpin", false);
        playerAnim.enabled = false;
    }
}
