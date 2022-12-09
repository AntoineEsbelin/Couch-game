using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EZCameraShake;
using EZVibrations;

public class Bumper : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 knockback;
    public float force;
    private GameObject player;
    private Rigidbody playerRb;
    private PlayerController playerCtrl;

    private Animator animate;

    private float timer;
    [SerializeField] private float maxTimer;

    private void Start()
    {
        timer = maxTimer;
        animate = GetComponent<Animator>();

    }

    private void Update()
    {
        if (player == null)
            return;
        
        //Timer();
    }
    
    private void Timer()
    {
        
        if(timer > 0)
        {
            if (playerCtrl.stopBumpKb)
            {
                playerCtrl.stopBumpKb = false;
                timer = 0;
                return;
            }

            timer -= Time.deltaTime;
            knockback = dir * force *(player.GetComponentInChildren<NormalState>().mSettings.moveSpeed * Time.deltaTime);

            playerRb.AddForce(knockback.x, 0f, knockback.z, ForceMode.Impulse);

        }
        
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            Vibrations.Instance.VibrateOnce(0.5f, 0.5f, PlayerInput.GetPlayerByIndex(0), 0.3f);
            player = col.gameObject;
            CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
            playerRb = player.GetComponent<Rigidbody>();
            playerCtrl = player.GetComponent<PlayerController>();

            StartCoroutine(AnimBumper());
            
            dir = transform.position - col.transform.position;
            dir.Normalize();

            dir = Vector3.Reflect(dir, col.contacts[0].normal);

            ////timer = maxTimer;
            playerCtrl.StunState.isKnockBacked = false;
            playerCtrl.stateMachine.SwitchState(playerCtrl.NormalState);
            playerCtrl.StunState.isKnockBacked = true;
            playerCtrl.StunState.kbSpeed = force;
            playerCtrl.StunState.timerMax = maxTimer;
            playerCtrl.StunState.timer = maxTimer;
            playerCtrl.StunState.kbDirBumper = -col.contacts[0].normal;
            // Debug.Log(-col.contacts[0].normal);
            // Debug.DrawRay(transform.position, -col.contacts[0].normal * 10, Color.red, 2f);
            playerCtrl.stateMachine.SwitchState(playerCtrl.StunState);
            
        }
    }

    IEnumerator AnimBumper()
    {
        if (animate == null) yield break;
        animate.SetBool("PlayerCol", true);
        yield return new WaitForSeconds(1f);
        animate.SetBool("PlayerCol", false);
    }
}

  
