using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCollision : MonoBehaviour
{

    public BounceWall bounceWall;
    public BounceSpinner bounceSpinner;
    public BouncePlayer bouncePlayer;
    public SpinnerControler spinnerControler;
    public PlayerManager playerManager;

    public float timer;

    void OnEnable()
    {
        bounceWall = GetComponent<BounceWall>();
        bounceSpinner = GetComponent<BounceSpinner>();
        bouncePlayer = GetComponent<BouncePlayer>();
        playerManager = GetComponent<PlayerManager>();
    }

    void OnDisable()
    {
        playerManager.ResetAllInteraction();
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
            
            if(spinnerControler.gameObject.activeSelf)
            {
                other.GetComponentInParent<PlayerManager>().lastPlayerContacted = this.GetComponent<PlayerManager>();
                other.GetComponentInParent<PlayerManager>().timeLastPlayer = other.GetComponentInParent<PlayerManager>().maxTimeLastPlayer;


                //Debug.Log(other.name);

                //Si le joueur est pas stun [???]

                if (other.gameObject.layer == 7)
                {
                    //activate bounce player of this spinner
                    this.bouncePlayer.enabled = true;

                    //activate knockback for triggered player >:(
                    other.GetComponentInParent<Knockback>().spinnerKnockbacking = this.spinnerControler;
                    other.GetComponentInParent<Knockback>().enabled = true;
                    
                }
                if (other.gameObject.layer == 8) bounceSpinner.enabled = true;
                
                /*if(spinnerControler.isSpinning)
                {
                }*/
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        
        if(other.gameObject.tag == "Wall")
        {
            if(spinnerControler.isSpinning)
            {
                //Debug.Log("Walled");
                bounceWall.normalizedWall = other.contacts[0].normal;
                bounceWall.playerDirection = spinnerControler.moveDir;
                timer = .5f;
                bounceWall.enabled = true;
            }
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
