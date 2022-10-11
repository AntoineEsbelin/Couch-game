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
            PlayerController deadPlayer = coll.GetComponent<PlayerController>();
            PlayerInput playerInput = coll.GetComponent<PlayerInput>();
            //Give a certain amount of point at the last player touched
            if(deadPlayer.playersInteract.lastPlayerContact != null)
            {
                deadPlayer.playersInteract.lastPlayerContact.playersInteract.playerPoint += pointGiven;
            
                //DEBUG
                Debug.Log($"{deadPlayer.name} EJECTED !");
                Debug.Log($"GIVE {pointGiven} points to {deadPlayer.playersInteract.lastPlayerContact.name}");

                if(GameManager.instance.drawTimer)
                {
                    if(deadPlayer.playersInteract.lastPlayerContact.playersInteract.playerPoint > GameManager.instance.drawMaxPoint)
                    {
                        Debug.Log($"{deadPlayer.playersInteract.lastPlayerContact.name} WINNN");
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
            //MORT DU JOUEUR TOMBÉ
                //Character Controller
                    //coll.GetComponent<ToupieBehaviour>().StartCoroutine(coll.GetComponent<ToupieBehaviour>().DeathState());
                //Rigidbody
                
        }
    }
}
