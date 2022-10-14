using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCollision : MonoBehaviour
{

    BounceWall bounceWall;
    BounceSpinner bounceSpinner;
    BouncePlayer bouncePlayer;
    public GameObject epxlosionParticle;

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
            other.GetComponentInParent<PlayerManager>().lastPlayerContacted = this.GetComponentInParent<PlayerManager>();
            other.GetComponentInParent<PlayerManager>().timeLastPlayer = other.GetComponentInParent<PlayerManager>().maxTimeLastPlayer;
            Instantiate(epxlosionParticle, this.transform.position, Quaternion.identity);
            if (other.gameObject.layer == 7) bouncePlayer.enabled = true;
            if (other.gameObject.layer == 8) bounceSpinner.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
            Debug.Log("Collision");
    
        if(other.gameObject.tag == "Wall")
        {
            Debug.Log("Walled");
            bounceWall.normalizedWall = other.contacts[0].normal;
            bounceWall.enabled = true;
            bounceWall.playerDirection = this.GetComponent<SpinnerControler>().moveDir;
        }
    }

    private void OnCollisionStay(Collision stay)
    {
        Debug.Log("ui");
    }

}
