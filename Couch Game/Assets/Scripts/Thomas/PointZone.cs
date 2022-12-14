using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EZCameraShake;
using EZVibrations;

public class PointZone : MonoBehaviour
{
    [Header("Point gagné quand marquage")]
    [SerializeField] private int pointGiven = 25;

    [Header("Point perdu quand suicide")]
    [SerializeField] private int pointRemoved = 5;
    [SerializeField] private bool isField;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int explosionMultiplier = 1;
    private GameObject followCam;
    public static bool isSettingSuicideOn;

    private PlayerInput controller;
    [Header("Vibration Controller")] 
    [Range(0.0f,1.0f)]
    public float LeftMotor;
    [Range(0.0f,1.0f)]
    public float RightMotor;
    private float VibroTimer;


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
            Vibrations.Instance.VibrateOnce(0.5f, 0.5f, PlayerInput.GetPlayerByIndex(0), 0.3f);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            if(!isField || deadPlayer.currentState == deadPlayer.DeathState)return;
            GameObject expl = Instantiate(explosion, deadPlayer.transform.position, Quaternion.identity);
            expl.transform.localScale *= 2;

            DispawnPlayer(deadPlayer);

        }
    }

    private void Update()
    {
          /*//controller vibration
          if (controller != null)
          {
              if(controller.GetDevice<Gamepad>() == null)return;
              Gamepad gamePad = controller.GetDevice<Gamepad>();
              

              if (gamePad == Gamepad.current)
              {
                  if (VibroTimer > 0)
                  {
                      VibroTimer -= Time.deltaTime;
                      
                      gamePad.SetMotorSpeeds(LeftMotor, RightMotor);
                      
                  }
                  else
                  {
                      gamePad.SetMotorSpeeds(0f, 0f);
                  }
                  
              }
          }*/
        
    }


    private void DispawnPlayer(PlayerController deadPlayer)
    {
        if (deadPlayer.currentState == deadPlayer.DeathState)return;
        deadPlayer.timeMultiplier = deadPlayer.maxtimeMultiplier + deadPlayer.DeathState.respawnTime;
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Goal"), this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        
        if(deadPlayer.lastPlayerContacted != null)
        {
            int point = (pointGiven * deadPlayer.multiplier);
            //Debug.Log(point);
            deadPlayer.lastPlayerContacted.playerPoint += point;
            deadPlayer.lastPlayerContacted.UpdateScore(pointGiven, false);
            //DEBUG
            //Debug.Log($"{deadPlayer.name} EJECTED !");
            //Debug.Log($"GIVE {(point)} points to {deadPlayer.lastPlayerContacted.name}");

            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Crowd Shouting"]/*.GetValueOrDefault("Crowd Shouting")*/, this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            if(GameManager.instance.allCrowd != null)GameManager.instance.Cheer();
            CheckBestPlayer();

            if(GameManager.instance.gameTimer.drawTimer)
            {
                PlayerController firstRankPlayer = GameManager.instance.PlayerRanking[0];
                if(firstRankPlayer.playerPoint > GameManager.instance.PlayerRanking[1].playerPoint)GameManager.instance.PlayerWin(firstRankPlayer);
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
            if (PointZone.isSettingSuicideOn)
            {
                deadPlayer.playerPoint -= pointRemoved;
                deadPlayer.UpdateScore(pointRemoved, true);
                CheckBestPlayer();
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
        
        GameManager.instance.UpdateRanking();
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
