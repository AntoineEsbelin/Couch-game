using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : PlayerState
{


    public Vector3 knockbackDir;
    public Vector3 kbDirBumper;
    private Vector3 knockback;
    public float kbSpeed;
    public bool isKnockBacked;

    [HideInInspector] public float timer;
    public float timerMax = 1f;

    public PlayerController stunplayer;

    private AudioSource sfx;


    public override void EnterState(PlayerController player)
    {
        playerController = player;
        timer = timerMax;
        if (isKnockBacked){knockbackDir = kbDirBumper;}
        if(player.lastPlayerContacted == null)return;
        stunplayer = player.lastPlayerContacted;
        playerController.isMoving = false;
        //if (isKnockBacked){knockbackDir = kbDirBumper;Debug.Log("bump");}
        if(!stunplayer.hasCountered)knockbackDir = /*(playerController.transform.position - stunplayer.transform.position).normalized*/ stunplayer.rb.velocity.normalized;
        //else if(isKnockBacked){knockbackDir = kbDirBumper;Debug.Log("bump");}
        else knockbackDir = Vector3.zero;
        Debug.Log($"Player Dir : {playerController.move }");
        Debug.Log($"Knockback Dir : {knockbackDir}");
        if (!isKnockBacked)
            kbSpeed = stunplayer.SpinnerState.mSettings.moveSpeed;
        if(!stunplayer.hasCountered)
        {
            knockbackDir = (stunplayer.rb == null ? stunplayer.rb.velocity.normalized : (playerController.transform.position - stunplayer.transform.position).normalized);
        }
        else
        {
            knockbackDir = Vector3.zero;
            
            stunplayer.hasCountered = false;
            playerController.PlayerAnimator.SetBool("IsStunned", true);

            playerController.rb.velocity = Vector3.zero;
            sfx = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Stun"], player.transform.position, AudioManager.instance.soundEffectMixer, true, true);
            //Debug.Log("NE BOUGE PAS");
        }
        Debug.Log($"Player Dir : {playerController.move }");
        Debug.Log($"Knockback Dir : {knockbackDir}");
        kbSpeed = stunplayer.SpinnerState.mSettings.moveSpeed;
    }

    public override void UpdateState(PlayerController player)
    {
        Timer();
        Knockback();
    }

    public override void ExitState(PlayerController player)
    {
        isKnockBacked = false;
        if(playerController.PlayerAnimator.GetBool("IsStunned"))playerController.PlayerAnimator.SetBool("IsStunned", false);
        if(player.SpinnerState.repoussed)
        {
            player.SpinnerState.repoussed = false;
            Debug.Log("EXIT");
        }
        if(sfx != null)Destroy(sfx.gameObject);
    }

    [System.Serializable] public class MovementSettings
    {
        
    }

    void Timer()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
        else
        playerController.stateMachine.SwitchState(playerController.NormalState);
    }

    void Knockbackdkjtgzksjhcgfkusxhr()
    {
        knockback = knockbackDir * ((kbSpeed *0.1f) * Time.fixedDeltaTime);
        playerController.rb.AddForce(knockback.x, 0f, knockback.z, ForceMode.Impulse);
    }

    void Knockback()
    {
        Debug.Log(knockbackDir);
        //float kbSmooth = (Mathf.Pow((timer / (timerMax - 1)), 3) + 1) * kbSpeed;
        knockback = knockbackDir * kbSpeed * Time.fixedDeltaTime;
        playerController.rb.velocity = new Vector3(knockback.x,0f,knockback.z);
        
    }
}
