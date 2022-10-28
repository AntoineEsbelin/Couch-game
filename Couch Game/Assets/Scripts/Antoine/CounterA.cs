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

    public int normalStun;
    public int spinStun;


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
            Vector3 forceToApply = orientation.forward * forceApplied;
            
             
            if (pm.isInSpinMode && pm != null)
            {
                norm.stunned = true;
                norm.stunDuration = spinStun;
                rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
                Debug.Log("YEE");
                
            }
            else if (!pm.isInSpinMode && pm != null)
            {
                norm.stunned = true;
                norm.stunDuration = normalStun;
                rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
            }

            
            //Debug.Log("CD applied");
            canAtk = false;
            yield return new WaitForSeconds(attackCD);
        }
        
    }

}
