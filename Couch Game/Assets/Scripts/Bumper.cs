using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 knockback;
    public float force;
    private GameObject player;

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
            timer -= Time.deltaTime;
            knockback = dir * force *(player.GetComponentInChildren<NormalState>().mSettings.moveSpeed * Time.deltaTime);

            player.GetComponent<Rigidbody>().AddForce(knockback.x, 0f, knockback.z, ForceMode.Impulse);
            
        }
        
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            player = col.gameObject;
            
            dir = transform.position - col.transform.position;
            dir.Normalize();

            dir = Vector3.Reflect(dir, col.contacts[0].normal);
            
            timer = maxTimer;
            
        }
    }
}

  
