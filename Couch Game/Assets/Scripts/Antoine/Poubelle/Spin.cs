using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    private Rigidbody rb;
    private PlayerManager pm;

    [Header("AttackStats")]
    public float attackCD;
    public float chargeForceApplied = 20;
    public bool canAtk;

    private void Awake()
    {      
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerManager>();
        canAtk = true;
       // hitbox = 
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
         Vector3 forceToApply = orientation.forward * chargeForceApplied;

         rb.AddForce(forceToApply, ForceMode.Impulse);

         canAtk = false;
                   
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCD);
        canAtk = true;
    }

}
