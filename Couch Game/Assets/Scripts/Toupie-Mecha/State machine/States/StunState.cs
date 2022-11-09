using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : PlayerState
{
    public override void EnterState(PlayerController player)
    {
        playerController = player;
        timer = timerMax;
        if(player.lastPlayerContacted != null) // A AMELIORER, SOLUTION TEMPORAIRE ZBI (pas utiliser lastplayermachin)
        {
            knockbackDir = (player.transform.position - player.lastPlayerContacted.transform.position).normalized;
            kbSpeed = player.lastPlayerContacted.SpinnerState.mSettings.moveSpeed;
        }
    }

    public override void UpdateState(PlayerController player)
    {
        Timer();
        Knockback();
    }

    public override void ExitState(PlayerController player)
    {
        
    }

    [System.Serializable] public class MovementSettings
    {
        
    }

    public MovementSettings mSettings;

    [HideInInspector] public Vector3 knockbackDir;
    private Vector3 knockback;
    float kbSpeed;

    float timer;
    public float timerMax = 1f;

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
