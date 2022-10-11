using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointZone : MonoBehaviour
{
    [SerializeField] private int pointGiven;

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        {
            PlayerController deadPlayer = coll.GetComponent<PlayerController>();
            //Give a certain amount of point at the last player touched
            if(deadPlayer.playersInteract.lastPlayerContact != null)
            {
                deadPlayer.playersInteract.lastPlayerContact.playersInteract.playerPoint += pointGiven;
            
                //DEBUG
                Debug.Log($"{deadPlayer.name} EJECTED !");
                Debug.Log($"GIVE {pointGiven} points to {deadPlayer.playersInteract.lastPlayerContact.name}");
            }
            else
            {
                //DEBUG
                Debug.Log($"{deadPlayer.name} SUICIDED !");
            }
            //MORT DU JOUEUR TOMBÉ

        }
    }
}
