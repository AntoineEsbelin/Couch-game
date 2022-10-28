using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class CounterA : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    public Rigidbody rb;
    public PlayerManager pm;
    public NormalControler norm;

    [Header("AttackStats")]
    public float attackCD = 3.0f;
    public float forceApplied = 20;
    public bool canAtk = false;

    private void Awake()
    {
        rb = null;
        //pm = GetComponent<PlayerManager>();
        hitbox = GameObject.FindGameObjectWithTag("hitbox").GetComponent<BoxCollider>();
        orientation = transform;
        canAtk = false;
    }
    private void FixedUpdate()
    {
        StartCoroutine(Attack());
    }

    public void OnStartAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            canAtk = true;
        }
    }

    public IEnumerator Attack()
    {
        if(canAtk && rb != null)
        {
            //Debug.Log("Attack");
            Vector3 forceToApply = orientation.forward * forceApplied;
            norm.stunned = true;
            norm.stunDuration = 1;

            // Quand les states seront l�. 
            //if (pm.spin)
            //{
            //    norm.stunned = true;
            //    norm.stunDuration = 2;
            //    rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
            //    norm.state = normal;

            //}

            rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
            yield return new WaitForSeconds(attackCD);
            Debug.Log("CD applied");
            canAtk = false;
        }
        
    }

}
