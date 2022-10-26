using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CounterA counterAtk;
    private void Awake()
    {
        counterAtk = GetComponentInParent<CounterA>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counterAtk.rb = other.attachedRigidbody;
            counterAtk.pm = other.attachedRigidbody.GetComponent<PlayerManager>();
            counterAtk.norm = other.attachedRigidbody.GetComponentInChildren<NormalControler>();
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
