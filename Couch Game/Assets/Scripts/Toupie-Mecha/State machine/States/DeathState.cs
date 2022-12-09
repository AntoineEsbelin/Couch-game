using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathState : PlayerState
{
    public float maxRespawnTime = 3f;
    public float respawnTime;
    public override void EnterState(PlayerController player)
    {
        PlayerController pCtrl = GetComponent<PlayerController>();
        pCtrl.NormalState.mSettings.glueSpeedModifier = 1f;
        pCtrl.SpinnerState.mSettings.glueSpeedModifier = 1f;

        player.GetComponent<CapsuleCollider>().enabled = false;
        player.GetComponent<SphereCollider>().enabled = false;
        //player.GetComponentInChildren<BoxCollider>().enabled = false;
        player.rb.velocity = Vector3.zero;
        player.ResetCharging();
        player.cameraTarget.targets.Remove(player.transform);
        respawnTime = maxRespawnTime;
        WallEvent wallEvent = GameObject.FindObjectOfType<WallEvent>();
        if(wallEvent != null)player.NeonBugBounce(wallEvent);
        //visuel off :
        ResetAnimator(player.PlayerAnimator);
       // player.playerFBX.SetActive(false);
        player.playerCrown.SetActive(false);
        player.GetComponentInChildren<SpinningAnim>(true).transform.localScale = Vector3.one;
        player.stopBumpKb = true;
        if(player.sfx != null)Destroy(player.sfx);
        if(player.SpinnerState.allSpinnerVFX.Count > 0)player.SpinnerState.RemoveAllSpinnerFX();
    }
    public override void UpdateState(PlayerController player)
    {
        if(respawnTime > 0)respawnTime -= Time.deltaTime;
        else player.stateMachine.SwitchState(player.NormalState); 
    }
    public override void ExitState(PlayerController player)
    {
        player.GetComponent<CapsuleCollider>().enabled = true;
        player.GetComponent<SphereCollider>().enabled = true;
        //player.GetComponentInChildren<BoxCollider>().enabled = true;
        //visuel on :
        player.cameraTarget.targets.Add(transform);
     //   player.PlayerAnimator.enabled = true;
      //  player.playerFBX.SetActive(true);
        player.RespawnPlayer();
        if(player.hasDaCrown)player.playerCrown.SetActive(true);
    }


    private void ResetAnimator(Animator playerAnim)
    {
        playerAnim.SetTrigger("Death");
        playerAnim.SetBool("IsWalking", false);
        playerAnim.SetBool("IsSpinning", false);
        playerAnim.SetBool("Counter", false);
        playerAnim.SetBool("ChargingSpin", false);
    }
}
