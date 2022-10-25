using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CounterA counterAtk;
    public Rigidbody rigid;
    private void Awake()
    {
        counterAtk = GetComponentInParent<CounterA>();
    }
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            counterAtk.canAtk = true;
            counterAtk.rb = collider.attachedRigidbody;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counterAtk.canAtk = false;
            counterAtk.rb = null;
        }
    }

}
