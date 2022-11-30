using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CounterA counterAtk;
    private void Awake()
    {
        counterAtk = GetComponent<CounterA>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerController>().lastPlayerContacted = GetComponent<PlayerController>();//ça marche pas à refaire (pb de collision point d'interrogation)
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            counterAtk.rb = rb;
            counterAtk.pm = rb.GetComponent<PlayerController>();
            if (other.GetComponentInParent<PlayerController>().playerId == this.GetComponent<PlayerController>().playerId) counterAtk.pm = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counterAtk.rb = null;
            counterAtk.pm = null;
            counterAtk.plctrl.hasCountered = false;
        }
    }

}
