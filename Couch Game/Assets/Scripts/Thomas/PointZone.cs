using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EZCameraShake;

public class PointZone : MonoBehaviour
{
    
    [SerializeField] private int pointGiven;
    [SerializeField] private bool isField;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int explosionMultiplier = 1;

    private float VibroTimer;
    [SerializeField] private float maxVibroTimer = 2f;
    private PlayerInput controller;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            controller = coll.GetComponent<PlayerInput>();
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            if(isField || deadPlayer.currentState == deadPlayer.DeathState)return;
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            GameObject expl = Instantiate(explosion, this.transform.position, Quaternion.identity);
            expl.transform.localScale *= explosionMultiplier;
            DispawnPlayer(deadPlayer);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            if(!isField || deadPlayer.currentState == deadPlayer.DeathState)return;
            GameObject expl = Instantiate(explosion, deadPlayer.transform.position, Quaternion.identity);
            expl.transform.localScale *= 2;

            DispawnPlayer(deadPlayer);

        }
    }

    private void FixedUpdate()
    {
        //controller vibration
        /*if (controller != null)
        {
            if(controller.GetDevice<Gamepad>() == null)return;
            Gamepad gamePad = controller.GetDevice<Gamepad>();

            if (gamePad == Gamepad.current)
            {
                if (VibroTimer > 0)
                {
                    VibroTimer -= Time.deltaTime;
                    
                    gamePad.SetMotorSpeeds(0.5f, 1.5f);
                    
                }
                
            }
        }*/
        
    }


    private void DispawnPlayer(PlayerController deadPlayer)
    {
        if(deadPlayer.currentState == deadPlayer.DeathState)return;
        //VibroTimer = maxVibroTimer; 
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Goal"), this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        
        if(deadPlayer.lastPlayerContacted != null)
        {
            deadPlayer.lastPlayerContacted.playerPoint += pointGiven;
            deadPlayer.lastPlayerContacted.UpdateScore(deadPlayer.lastPlayerContacted.playerPoint);
            //DEBUG
            Debug.Log($"{deadPlayer.name} EJECTED !");
            Debug.Log($"GIVE {pointGiven} points to {deadPlayer.lastPlayerContacted.name}");

            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Crowd Shouting"]/*.GetValueOrDefault("Crowd Shouting")*/, this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            
            if(GameManager.instance.gameTimer.drawTimer)
            {
                if(deadPlayer.lastPlayerContacted.playerPoint > GameManager.instance.gameTimer.drawMaxPoint)
                {
                    GameManager.instance.PlayerWin(deadPlayer.lastPlayerContacted);
                    //STOP THE ROUND
                }
            }
            else
            {
                int randomPraise = Random.Range(0, 8);
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Voice Praise {randomPraise}"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            }
        }
        else
        {
            //DEBUG
            Debug.Log($"{deadPlayer.name} SUICIDED !");
        }
        if(deadPlayer.gameObject.activeSelf)
        {
            
            deadPlayer.stateMachine.SwitchState(deadPlayer.DeathState);
        }
        
        
    }
}
