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
    private GameObject followCam;
    public static bool isSettingSuicideOn;

    private void Start()
    {
        followCam = GameObject.Find("followCam");
    }

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
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
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
        deadPlayer.timeMultiplier = deadPlayer.maxtimeMultiplier + deadPlayer.DeathState.respawnTime;
        //VibroTimer = maxVibroTimer; 
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Goal"), this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        
        if(deadPlayer.lastPlayerContacted != null)
        {
            int point = (pointGiven * deadPlayer.multiplier);
            //Debug.Log(point);
            deadPlayer.lastPlayerContacted.playerPoint += point;
            deadPlayer.lastPlayerContacted.UpdateScore(deadPlayer.lastPlayerContacted.playerPoint);
            //DEBUG
            //Debug.Log($"{deadPlayer.name} EJECTED !");
            //Debug.Log($"GIVE {(point)} points to {deadPlayer.lastPlayerContacted.name}");

            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Crowd Shouting"]/*.GetValueOrDefault("Crowd Shouting")*/, this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            if(GameManager.instance.allCrowd != null)GameManager.instance.Cheer();
            CheckBestPlayer();

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
                int randomPraise = Random.Range(0, 7);
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Voice Praise {randomPraise + 1}"], this.transform.position, AudioManager.instance.announcerMixer, true, false);
            }
        }
        else
        {
            //DEBUG
            Debug.Log($"{deadPlayer.name} SUICIDED !");
            if (isSettingSuicideOn)
            {
                deadPlayer.playerPoint -= 1;
                deadPlayer.UpdateScore(deadPlayer.playerPoint);
            }

        }
        if(deadPlayer.gameObject.activeSelf)
        {
            
            deadPlayer.stateMachine.SwitchState(deadPlayer.DeathState);
        }
        
        
    }

    private void CheckBestPlayer()
    {
        int playerPoint = 0;
        //get max point + player
        for(int i = 0; i < GameManager.instance.allPlayer.Count; i++)
        {
            if(GameManager.instance.allPlayer[i].playerPoint >= playerPoint)
            {
                playerPoint = GameManager.instance.allPlayer[i].playerPoint;
            }
        }
        
        for(int i = 0; i < GameManager.instance.allPlayer.Count; i++)
        {
            if(GameManager.instance.allPlayer[i].playerPoint >= playerPoint)
            {
                if(GameManager.instance.allPlayer[i].hasDaCrown == true)return;
                GameManager.instance.allPlayer[i].hasDaCrown = true;
                GameManager.instance.allPlayer[i].playerCrown.SetActive(true);
                followCam.GetComponent<FollowPlayer>().follow(GameManager.instance.allPlayer[i].gameObject);
            }
            else
            {
                GameManager.instance.allPlayer[i].hasDaCrown = false;
                GameManager.instance.allPlayer[i].playerCrown.SetActive(false);
            }
        }
    }
}
