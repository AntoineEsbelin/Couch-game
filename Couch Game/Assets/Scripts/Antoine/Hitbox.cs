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
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            counterAtk.rb = rb;
            counterAtk.pm = rb.GetComponent<PlayerManager>();
            counterAtk.norm = counterAtk.pm.normalPlayer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counterAtk.rb = null;
            counterAtk.pm = null;
            counterAtk.norm = null;
        }
    }

}
