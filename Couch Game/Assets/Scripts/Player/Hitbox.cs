using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CounterA counterAtk;
    private void Awake()
    {
        counterAtk = GetComponent<CounterA>();
        //Debug.Log(this.GetComponentInChildren<BoxCollider>().center);
        //Debug.Log(this.GetComponentInChildren<BoxCollider>().size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController otherPlayer = other.GetComponentInParent<PlayerController>();
            otherPlayer.lastPlayerContacted = GetComponentInParent<PlayerController>();//ça marche pas à refaire (pb de collision point d'interrogation)
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            counterAtk.rb = rb;
            counterAtk.pm = rb.GetComponent<PlayerController>();
            //if(otherPlayer.startCharging)otherPlayer.ResetCharging();            
            if (otherPlayer.playerId == this.GetComponent<PlayerController>().playerId) counterAtk.pm = null;
            //Debug.Log("ENTER IN TRIGGER");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counterAtk.rb = null;
            counterAtk.pm = null;
            //counterAtk.plctrl.hasCountered = false;
            //Debug.Log("LEAVING TRIGGER");

        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(this.GetComponentInChildren<BoxCollider>().gameObject.transform.position, this.GetComponentInChildren<BoxCollider>().size);
    }
}
