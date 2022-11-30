using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointZone : MonoBehaviour
{
    
    [SerializeField] private int pointGiven;
    [SerializeField] private bool isField;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int explosionMultiplier = 1;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            if(isField)return;
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            GameObject expl = Instantiate(explosion, this.transform.position, Quaternion.identity);
            expl.transform.localScale *= explosionMultiplier;
            DispawnPlayer(deadPlayer);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            if(!isField)return;
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            GameObject expl = Instantiate(explosion, deadPlayer.transform.position, Quaternion.identity);
            expl.transform.localScale *= 2;
            DispawnPlayer(deadPlayer);

        }
    }


    private void DispawnPlayer(PlayerController deadPlayer)
    {
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Goal"), this.transform.position, AudioManager.instance.soundEffectMixer, true);
        if(deadPlayer.lastPlayerContacted != null)
        {
            deadPlayer.lastPlayerContacted.playerPoint += pointGiven;
            deadPlayer.lastPlayerContacted.UpdateScore(deadPlayer.lastPlayerContacted.playerPoint);
            //DEBUG
            Debug.Log($"{deadPlayer.name} EJECTED !");
            Debug.Log($"GIVE {pointGiven} points to {deadPlayer.lastPlayerContacted.name}");

            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Crowd Shouting"]/*.GetValueOrDefault("Crowd Shouting")*/, this.transform.position, AudioManager.instance.soundEffectMixer, true);
            
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
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Voice Praise {randomPraise}"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
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
