using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCollision : MonoBehaviour
{

    public BounceWall bounceWall;
    public BounceSpinner bounceSpinner;
    public BouncePlayer bouncePlayer;
    public GameObject epxlosionParticle;
    public SpinnerControler spinnerControler;

    public float timer;

    void OnEnable()
    {
        bounceWall = GetComponent<BounceWall>();
        bounceSpinner = GetComponent<BounceSpinner>();
        bouncePlayer = GetComponent<BouncePlayer>();
    }

    private void FixedUpdate()
    {
        BounceWallTimer();
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
    
        if(other.gameObject.tag == "Wall")
        {
            Debug.Log("Walled");
            bounceWall.normalizedWall = other.contacts[0].normal;
            bounceWall.playerDirection = spinnerControler.moveDir;
            timer = .5f;
            bounceWall.enabled = true;
        }
    }

    private void BounceWallTimer()
    {
        if(bounceWall.spinerControler.walled)
        {
            if(timer > 0)timer -=Time.deltaTime;
            else bounceWall.spinerControler.walled = false;
        }
    }
}
