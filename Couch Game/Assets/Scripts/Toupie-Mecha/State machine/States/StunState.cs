using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : PlayerState
{

    public MovementSettings mSettings;

     public Vector3 knockbackDir;
    private Vector3 knockback;
    float kbSpeed;

    float timer;
    public float timerMax = 1f;

    public PlayerController stunplayer;

    private AudioSource sfx;


    public override void EnterState(PlayerController player)
    {
        playerController = player;
        timer = timerMax;
        if(player.lastPlayerContacted == null)return;
        stunplayer = player.lastPlayerContacted;
        playerController.isMoving = false;
        if(!stunplayer.hasCountered)knockbackDir = /*(playerController.transform.position - stunplayer.transform.position).normalized*/ stunplayer.rb.velocity.normalized;
        else knockbackDir = Vector3.zero;
        Debug.Log($"Player Dir : {playerController.move }");
        Debug.Log($"Knockback Dir : {knockbackDir}");
        kbSpeed = stunplayer.SpinnerState.mSettings.moveSpeed;
        if(stunplayer.hasCountered)
        {
            stunplayer.hasCountered = false;
            playerController.PlayerAnimator.SetBool("IsStunned", true);

            playerController.rb.velocity = Vector3.zero;
            sfx = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Stun"], player.transform.position, AudioManager.instance.soundEffectMixer, true, true);
            //Debug.Log("NE BOUGE PAS");
        }
    }

    public override void UpdateState(PlayerController player)
    {
        Timer();
        Knockback();
    }

    public override void ExitState(PlayerController player)
    {
        
        playerController.PlayerAnimator.SetBool("IsStunned", false);
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
        knockback = knockbackDir * ((kbSpeed) * Time.fixedDeltaTime);
        playerController.rb.velocity = new Vector3(knockback.x,0f,knockback.z);
        
    }
}
