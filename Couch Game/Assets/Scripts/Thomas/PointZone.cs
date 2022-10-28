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
            PlayerManager deadPlayer = coll.GetComponentInParent<PlayerManager>();
            DispawnPlayer(deadPlayer);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            if(!isField)return;
            PlayerManager deadPlayer = coll.GetComponentInParent<PlayerManager>();
            DispawnPlayer(deadPlayer);

        }
    }


    private void DispawnPlayer(PlayerManager deadPlayer)
    {
        if(deadPlayer.normalPlayer.gameObject.activeSelf)
        {
            deadPlayer.normalPlayer.gameObject.SetActive(false);
            deadPlayer.normalPlayer.gameObject.GetComponent<NormalControler>().enabled = false;
        }
        else if(deadPlayer.spinnerPlayer.activeSelf)
        {
            deadPlayer.spinnerPlayer.SetActive(false);
            deadPlayer.spinnerControler.GetComponent<SpinnerControler>().enabled = false;
        }
        deadPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        deadPlayer.ResetCharging();
        deadPlayer.cameraTarget.targets.Remove(deadPlayer.transform);
        //Give a certain amount of point at the last player touched
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
        deadPlayer.GetComponent<PlayerRespawn>().enabled = true;
        deadPlayer.enabled = false;
    }
}
