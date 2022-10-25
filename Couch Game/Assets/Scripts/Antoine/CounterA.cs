using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterA : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    public Rigidbody rb;
    private PlayerManager pm;

    [Header("AttackStats")]
    public float attackCD;
    public float forceApplied = 20;
    public bool canAtk;

    private void Awake()
    {      
        pm = GetComponent<PlayerManager>();
        canAtk = false;
        hitbox = GameObject.FindGameObjectWithTag("hitbox").GetComponent<BoxCollider>();
        orientation = transform;
    }

    private void FixedUpdate()
    {       

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canAtk)
            {
                Attack();
            }
        }

    }

    public void Attack()
    {      
        
         Debug.Log("Attack");
         Vector3 forceToApply = orientation.forward * forceApplied;


        rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
        
         //canAtk = false;
                   
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCD);
        canAtk = true;
    }

}
