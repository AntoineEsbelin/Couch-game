using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 knockback;
    public float force;
    private GameObject player;
    private Rigidbody playerRb;
    private PlayerController playerCtrl;

    private Animator animate;

    private float timer;
    [SerializeField] private float maxTimer;

    private void Start()
    {
        timer = maxTimer;

    }

    private void Update()
    {
        if (player == null)
            return;
        
        Timer();
    }
    
    private void Timer()
    {
        
        if(timer > 0)
        {
            if (playerCtrl.stopBumpKb)
            {
                playerCtrl.stopBumpKb = false;
                timer = 0;
                return;
            }

            timer -= Time.deltaTime;
            knockback = dir * force *(player.GetComponentInChildren<NormalState>().mSettings.moveSpeed * Time.deltaTime);

            playerRb.AddForce(knockback.x, 0f, knockback.z, ForceMode.Impulse);

        }
        
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            player = col.gameObject;
            playerRb = player.GetComponent<Rigidbody>();
            playerCtrl = player.GetComponent<PlayerController>();
            animate = GetComponent<Animator>();

            StartCoroutine(AnimBumper());
            
            dir = transform.position - col.transform.position;
            dir.Normalize();

            dir = Vector3.Reflect(dir, col.contacts[0].normal);
            
            timer = maxTimer;
            
        }
    }

    IEnumerator AnimBumper()
    {
        animate.SetBool("PlayerCol", true);
        yield return new WaitForSeconds(1f);
        animate.SetBool("PlayerCol", false);
    }
}

  
