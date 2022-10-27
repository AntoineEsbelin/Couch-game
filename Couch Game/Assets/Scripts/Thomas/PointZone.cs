using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointZone : MonoBehaviour
{
    [SerializeField] private int pointGiven;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            PlayerManager deadPlayer = coll.GetComponentInParent<PlayerManager>();
            if(deadPlayer.normalPlayer.activeSelf)
            {
                deadPlayer.normalPlayer.SetActive(false);
                deadPlayer.normalPlayer.GetComponent<NormalControler>().enabled = false;
            }
            else if(deadPlayer.spinnerPlayer.activeSelf)
            {
                deadPlayer.spinnerPlayer.SetActive(false);
                deadPlayer.spinnerControler.GetComponent<SpinnerControler>().enabled = false;
            }
            deadPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            deadPlayer.cameraTarget.targets.Remove(deadPlayer.transform);
            //Give a certain amount of point at the last player touched
            if(deadPlayer.lastPlayerContacted != null)
            {
                deadPlayer.lastPlayerContacted.playerPoint += pointGiven;
            
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
}
