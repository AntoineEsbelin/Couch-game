using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCollision : MonoBehaviour
{

    BounceWall bounceWall;
    BounceSpinner bounceSpinner;
    BouncePlayer bouncePlayer;

    void Start()
    {
        bounceWall = GetComponent<BounceWall>();
        bounceSpinner = GetComponent<BounceSpinner>();
        bouncePlayer = GetComponent<BouncePlayer>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) return;
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.layer == 7) bouncePlayer.enabled = true;
            if (other.gameObject.layer == 8) bounceSpinner.enabled = true;
        }
    }

}
