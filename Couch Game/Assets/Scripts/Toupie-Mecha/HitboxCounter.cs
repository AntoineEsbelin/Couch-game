using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCounter : MonoBehaviour
{
    public CounterA counter;

    void Start()
    {
        //Debug.Log("paf");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("yeah");
            counter.pm = other.GetComponent<PlayerController>();
            counter.hasHit = true;
            counter.hitbox = this.GetComponent<BoxCollider>();
            counter.AtkCounter();
        }
    }
}
