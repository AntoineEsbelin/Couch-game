using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointZone : MonoBehaviour
{
    
    [SerializeField] private int pointGiven;
    [SerializeField] private bool isField;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            if(isField)return;
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            DispawnPlayer(deadPlayer);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            if(!isField)return;
            PlayerController deadPlayer = coll.GetComponentInParent<PlayerController>();
            DispawnPlayer(deadPlayer);

        }
    }


    private void DispawnPlayer(PlayerController deadPlayer)
    {
        if(deadPlayer.lastPlayerContacted != null)
        {
            deadPlayer.lastPlayerContacted.playerPoint += pointGiven;
            deadPlayer.lastPlayerContacted.UpdateScore(deadPlayer.lastPlayerContacted.playerPoint);
            //DEBUG
            Debug.Log($"{deadPlayer.name} EJECTED !");
            Debug.Log($"GIVE {pointGiven} points to {deadPlayer.lastPlayerContacted.name}");

            if(GameManager.instance.drawTimer)
            {
                if(deadPlayer.lastPlayerContacted.playerPoint > GameManager.instance.drawMaxPoint)
                {
                    Debug.Log($"{deadPlayer.lastPlayerContacted.name} WINNN");
                    GameManager.instance.timeOut = true;
                    //STOP THE ROUND
                }
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
